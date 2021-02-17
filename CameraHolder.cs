using Godot;
using System;
using FPSDemo;

public class CameraHolder : FPSCamera
{
	[Signal]
	public delegate void RotateY(float deg);
	[Signal]
	public delegate void RotateZ(float deg);
	[Signal]
	public delegate void Move(Vector3 vec);
}
