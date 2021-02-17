using Godot;

public class PhysicalEffect : KinematicBody
{
	public float YSpeed;

	public override void _Ready()
	{
		SetPhysicsProcess(true);
	}

	public override void _PhysicsProcess(float delta)
	{
		if (IsOnFloor()) return;
		YSpeed -= 9.8f * delta;
		MoveAndSlide(
			new Vector3(0, YSpeed, 0),
			Vector3.Up
		);
	}
}
