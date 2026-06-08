
import warnings
import os

os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'
os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'

warnings.filterwarnings('ignore', category=UserWarning)
warnings.filterwarnings('ignore', category=DeprecationWarning)
warnings.filterwarnings('ignore', category=FutureWarning)

import sys
import socket
import json
import random
import numpy as np
from collections import deque
from enum import Enum

import tensorflow as tf
tf.get_logger().setLevel('ERROR')
from tensorflow.keras import models, layers, optimizers # type: ignore

#---------------------------------------------------------------------------------------------------------------------------------------

class ReplayBuffer:

    def __init__(self, capacity = 50000):
        self.buffer = deque(maxlen = capacity)

    def add(self, state, action, reward, next_state, done):
        self.buffer.append((state, action, reward, next_state, done))

    def sample(self, batch_size):
        batch = random.sample(self.buffer, batch_size)
        states, actions, rewards, next_states, dones = zip(*batch)

        return np.array(states, dtype=np.float32), np.array(actions, dtype=np.int32), np.array(rewards, dtype=np.float32), np.array(next_states, dtype=np.float32), np.array(dones, dtype=np.float32)

    def __len__(self):
        return len(self.buffer)


replay_buffer = ReplayBuffer()

#---------------------------------------------------------------------------------------------------------------------------------------

ACTIONS = [-1, 0, 1]

step = 0

epsilon = 1.0
epsilon_min = 0.05
epsilon_step = 0.0001

gamma = 0.9

batch_size = 64
target_update_freq = 2000

exp_a_new = 0.4
exp_a_old = 0.6


def create_ddqn_model():  
    model = models.Sequential( [ layers.Input(shape = (13, )),  layers.Dense(64, activation = 'tanh'), layers.Dense(64, activation = 'tanh'),
        layers.Dense(9, activation = 'linear') ])

    model.compile(optimizer = optimizers.Adam(learning_rate = 0.0005, clipnorm = 1.0), loss = tf.keras.losses.Huber())

    return model


ddqn_main = create_ddqn_model()
ddqn_target = create_ddqn_model()

#---------------------------------------------------------------------------------------------------------------------------------------

MainWeightsFile = "Main.weights.h5"
TargetWeightsFile = "Target.weights.h5"


def load_weights():
    ddqn_main.load_weights(MainWeightsFile)
    ddqn_target.load_weights(TargetWeightsFile)

    print("\nВеса загрузились.\n")

def save_weights():
    ddqn_main.save_weights(MainWeightsFile)
    ddqn_target.save_weights(TargetWeightsFile)

    print("\n\nВеса сохранены.")

#---------------------------------------------------------------------------------------------------------------------------------------

DDASaveFile = "DDA_Save.json"


def load_data():
    with open(DDASaveFile, "r") as f:
        data = json.load(f)

    step_ = data["step"]
    epsilon_ =  data["epsilon"]

    for item in data["replay_buffer"]:
        state_ = tuple(item["state"])
        action_idx_ = item["action"]
        reward_ = item["reward"]
        next_state_ = tuple(item["next_state"])
        done_ = item["done"]

        replay_buffer.add(state_, action_idx_, reward_, next_state_, done_)

    print("\nДанные загружены.\n")

    return step_, epsilon_

def save_data():
    data = {
        "step": int(step),
        "epsilon": float(epsilon),
        "replay_buffer": [{
            "state": [float(x) for x in state],
            "action": int(action),
            "reward": float(reward),
            "next_state": [float(x) for x in next_state],
            "done": bool(done)
        }
        for (state, action, reward, next_state, done)
        in list(replay_buffer.buffer)]
    }

    with open(DDASaveFile, "w") as f:
        json.dump(data, f, indent=2)

    print("\nДанные сохранены.\n")

#---------------------------------------------------------------------------------------------------------------------------------------

# idx | platform | enemy
# -----------------------
# 0   |   -1     |  -1
# 1   |   -1     |   0
# 2   |   -1     |  +1
#
# 3   |    0     |  -1
# 4   |    0     |   0
# 5   |    0     |  +1
#
# 6   |   +1     |  -1
# 7   |   +1     |   0
# 8   |   +1     |  +1

def get_action_platform(index):
    return ACTIONS[int(index) // 3]

def get_action_enemy(index):
    return ACTIONS[index % 3]

def get_action(state):
    return np.argmax(ddqn_main.predict(np.array([state]), verbose=0)[0])

def get_action_with_epsilon(state):
    global epsilon

    if np.random.rand() <= epsilon:
        action_idx_ = random.randint(0, 8)
    else:
        action_idx_ = get_action(state)

    epsilon = max(epsilon_min, epsilon - epsilon_step)  

    return action_idx_

def get_valid_idx(idx, diff_p, diff_e): 
    helper = diff_p + get_action_platform(idx)

    if helper > 4:
        idx -= 3
    elif helper < 0:
        idx += 3

    helper = diff_e + get_action_enemy(idx)

    if helper > 4:
        idx -= 1
    elif helper < 0:
        idx += 1

    return idx

#---------------------------------------------------------------------------------------------------------------------------------------

max_death = 8
max_time = 120

divisors = np.array([
    max_time,   # time
    max_time,   # ema_time
    max_time,   # delta_time
    max_death,  # platform_deaths
    max_death,  # ema_platform_deaths
    max_death,  # delta_platform_deaths
    1.0,        # act_plat
    4.0,        # diff_platform
    max_death,  # enemy_deaths
    max_death,  # ema_enemy_deaths
    max_death,  # delta_enemy_deaths
    1.0,        # act_enemy
    4.0,        # diff_enemy
], dtype=np.float32)

indices1 = [ 0, 1, 3, 4, 8, 9]
indices2 = [2, 5, 10]


def norm_state(state_):
    res = np.array(state_, dtype=np.float32) / divisors

    res[indices1] = np.clip(res[indices1], 0.0, 1.0)
    res[indices2] = np.clip(res[indices2], -1.0, 1.0)

    return res.astype(np.float32)

def write_state(state_):
    print(
        f"\nШАГ {step}"
        f"\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
        f"\nВРЕМЯ                   : {state[0]:.2f}"
        f"\nЭСС ВРЕМЕНИ             : {state[1]:.2f}"
        f"\nДЕЛЬТА ВРЕМЕНИ          : {state[2]:.2f}"
        f"\n"
        f"\nСМЕРТИ от ПЛАТФ.        : {state[3]}"
        f"\nЭСС СМЕРТИ от ПЛАТФ     : {state[4]:.2f}"
        f"\nДЕЛЬТА СМЕРТЕЙ от ПЛАТФ.: {state[5]}"
        f"\nДЕЙСТВИЕ ПЛАТФ.         : {state[6]}"
        f"\nСЛОЖНОСТЬ ПЛАТФ.        : {state[7]}"
        f"\n"
        f"\nСМЕРТИ от ВРАГОВ        : {state[8]}"
        f"\nЭСС СМЕРТИ от ВРАГОВ    : {state[9]:.2f}"
        f"\nДЕЛЬТА СМЕРТЕЙ от ВРАГОВ: {state[10]}"
        f"\nДЕЙСТВИЕ ВРАГИ          : {state[11]}"
        f"\nСЛОЖНОСТЬ ВРАГОВ        : {state[12]}"
        f"\n"
        f"\nФЛАГ ЗАВЕРШЕНИЯ         : {done}"
    )

#---------------------------------------------------------------------------------------------------------------------------------------

TARGET_TIME = 60
TARGET_DEATHS = 3

TARGET_TIME_LOW = 55
TARGET_TIME_HIGH = 65

TIME_LOW_BOUND = 45
TIME_HIGH_BOUND = 80

ABS_LOW_BOUND = 35
ABS_HIGH_BOUND = 95

TIME_REWARD_SCALE = 2.5
MIN_TIME_REWARD = -2.5

DEATH_REWARD_TABLE = {
    3: 2.5,

    4: 1.5,
    2: 1.5,

    1: 2.5,
    5: 0.5,

    0: -0.5,
    6: -0.5,

    7: -1.5,
}

DEATH_DEFAULT_REWARD = -2.5

TIME_SCALE = 0.12
DEATH_SCALE = 0.25

STABILITY_FLIP_PENALTY = 0.35

DELTA_TIME_PENALTY = 0.015
DELTA_DEATH_PENALTY = 0.03

LOW_DIFFICULTY_PENALTY = 0.06
LOW_DIFFICULTY_PENALTY_SCALE = 0.08

MAX_DIFF = 4

REWARD_MIN = -5.0
REWARD_MAX = 5.0


def get_reward(prev_state, curr_state, prev_idx, curr_idx):
    reward = 0.0

    seconds = curr_state[0]
    deaths = curr_state[3] + curr_state[8]

    if TARGET_TIME_LOW <= seconds <= TARGET_TIME_HIGH:
        reward += TIME_REWARD_SCALE
    elif TIME_LOW_BOUND <= seconds < TARGET_TIME_LOW:
        reward += ((seconds - TIME_LOW_BOUND) / (TARGET_TIME_LOW - TIME_LOW_BOUND)) * TIME_REWARD_SCALE
    elif TARGET_TIME_HIGH < seconds <= TIME_HIGH_BOUND:
        reward += ((TIME_HIGH_BOUND - seconds)/ (TIME_HIGH_BOUND - TARGET_TIME_HIGH)) * TIME_REWARD_SCALE
    elif seconds < TIME_LOW_BOUND:
        reward += ((TIME_LOW_BOUND - max(seconds, ABS_LOW_BOUND))/ (TIME_LOW_BOUND - ABS_LOW_BOUND)) * MIN_TIME_REWARD
    else:
        reward += ((min(seconds, ABS_HIGH_BOUND) - TIME_HIGH_BOUND) / (ABS_HIGH_BOUND - TIME_HIGH_BOUND)) * MIN_TIME_REWARD

    reward += DEATH_REWARD_TABLE.get(deaths, DEATH_DEFAULT_REWARD)

    reward += ((abs(prev_state[0] - TARGET_TIME) - abs(curr_state[0] - TARGET_TIME)) * TIME_SCALE)
    reward += ((abs(prev_state[3] + prev_state[8] - TARGET_DEATHS) - abs(curr_state[3] + curr_state[8] - TARGET_DEATHS)) * DEATH_SCALE)

    plat_action = get_action_platform(curr_idx)
    prev_plat_action = get_action_platform(prev_idx)

    if plat_action != 0 and plat_action == -prev_plat_action:
        reward -= STABILITY_FLIP_PENALTY

    enemy_action = get_action_enemy(curr_idx)
    prev_enemy_action = get_action_enemy(prev_idx)

    if enemy_action != 0 and enemy_action == -prev_enemy_action:
        reward -= STABILITY_FLIP_PENALTY

    reward -= (MAX_DIFF - curr_state[7]) * LOW_DIFFICULTY_PENALTY_SCALE
    reward -= (MAX_DIFF - curr_state[12]) * LOW_DIFFICULTY_PENALTY_SCALE

    reward -= abs(curr_state[2]) * DELTA_TIME_PENALTY
    reward -= (abs(curr_state[5]) + abs(curr_state[10])) * DELTA_DEATH_PENALTY

    reward = np.clip(reward, REWARD_MIN, REWARD_MAX)

    return reward

#---------------------------------------------------------------------------------------------------------------------------------------

class SocketStatus(Enum):
    OK = 1
    RETRY = 2
    EXIT = 3


def parse_socket_state(message):
    message = message.decode('utf-8').strip()

    if not message:
        return SocketStatus.RETRY, None, None, None, None, None
    if message == "exit":
        return SocketStatus.EXIT, None, None, None, None, None

    try:
        parts = message.split('_')

        if len(parts) != 29:
            return SocketStatus.RETRY, None, None, None, None, None

        values = list(map(float, parts))

        for v in values:
            if np.isnan(v) or np.isinf(v):
                return SocketStatus.RETRY, None, None, None, None, None

        idx = 0

        state = values[idx:idx + 13]
        idx += 13

        done = bool(values[idx])
        idx += 1

        prev_action_idx = int(values[idx])
        idx += 1

        action_idx = int(values[idx])
        idx += 1

        prev_raw_state = values[idx:idx + 13]

        return SocketStatus.OK,state, done, prev_action_idx, action_idx, prev_raw_state

    except Exception:
        return SocketStatus.RETRY, None, None, None, None, None

#---------------------------------------------------------------------------------------------------------------------------------------

is_release = False
is_debug = False

prev_action_idx = 4
action_idx = 4

prev_raw_state = None

# Example: cd (DiskLetter):\Way\To\File && python LearningDDA.py

if __name__ == '__main__':
    load_weights()

    if not is_release:
        step, epsilon = load_data()

    print("\nПодготовка...\n")

    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind(('127.0.0.1', 5005))
    server_socket.listen(1)
    print("\nОжидание соединения с игрой...\n")

    socket_obj, addres_info = server_socket.accept()
    print(f"\nСоединение установлено.\n")

    with socket_obj:
        while True:
            try:
                status, state, done, prev_action_idx, action_idx, prev_raw_state = parse_socket_state(socket_obj.recv(1024))

                match status:
                    case SocketStatus.RETRY:
                        socket_obj.sendall(("Retry\n").encode('utf-8'))
                        continue
                    case SocketStatus.EXIT:
                        break

                if is_debug:
                    write_state(state)

                diff_plat = state[7] 
                diff_enemy = state[12]

                if is_release:
                     action_idx = get_valid_idx(get_action(norm_state(state)), diff_plat, diff_enemy)
                else:
                    reward = get_reward(prev_raw_state, state, prev_action_idx, action_idx)
                    prev_raw_state = state
                    state = norm_state(state) 
                    replay_buffer.add(norm_state(prev_raw_state), action_idx, reward, state, done)

                    if step % target_update_freq == 0:
                        ddqn_target.set_weights(ddqn_main.get_weights())
                      
                    if step % 4 == 0 and len(replay_buffer) >= batch_size:
                        states, actions, rewards, next_states, dones = replay_buffer.sample(batch_size)

                        q_next_main = ddqn_main.predict(next_states, verbose=0)
                        q_next_target = ddqn_target.predict(next_states, verbose=0)
                        q_main = ddqn_main.predict(states, verbose=0)

                        for i in range(batch_size):
                            target = rewards[i]

                            if dones[i] < 0.5:
                                best_next_action = np.argmax(q_next_main[i])
                                target += gamma * q_next_target[i][best_next_action]

                            q_main[i][actions[i]] = target

                        ddqn_main.fit(states, q_main, epochs=1, verbose=0)

                    step += 1
                    action_idx = get_valid_idx(get_action_with_epsilon(state), diff_plat, diff_enemy)

                socket_obj.sendall(f"{get_action_platform(action_idx)}_{get_action_enemy(action_idx)}\n".encode('utf-8'))

            except Exception as e:
                socket_obj.sendall(("Error\n").encode('utf-8'))
                print("\nОшибка: ", e, "\n")
                break

    #---------------------------------------

    server_socket.close()  

    save_weights()
    save_data()
