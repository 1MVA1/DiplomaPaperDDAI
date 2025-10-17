
public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public interface IAdjustableDifficulty {
    void ApplyDifficulty(Difficulty difficulty);
}