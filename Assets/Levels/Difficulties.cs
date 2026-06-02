
using UnityEngine;


public enum Diff
{
    Easy,
    Medium,
    Hard
}

public interface IApplyDiff_Spike
{
    void ApplyDiff(Diff diff_, float size_, float rotation_);
}
public interface IApplyDiff_Saw
{
    void ApplyDiff(Diff diff_, float size_, float speed_, float timeStop_, float acceleration_, float maxSpeed_,
        Vector2[] points_, bool isCycle_);
}
public interface IApplyDiff_VoltZone
{
    void ApplyDiff(Diff diff_, float size_, bool isHorizontal_);
}
public interface IApplyDiff_Enemy
{
    void ApplyDiff(Diff diff_, bool isMovingRight_);
}

public interface IRefreshable
{
    void PrepDiff();

    void Refresh();
    void TurnOn();
    void TurnOff();
}
