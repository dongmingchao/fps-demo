module FPSDemo.Bullet

open Godot

type Class() =
  inherit KinematicBody()
  let mutable YSpeed = 0.0f
  let mutable Active = true
  let gravity = 0.2f
  let mutable speed = 0.0f
  let mutable hited = false
  member val InitialSpeed = 1000f with get, set
  member this.Speed
    with get () = speed
    and set (value) = speed <- value
  override this._Ready() =
    this.SetAsToplevel(true)
    this.SetPhysicsProcess(true)

  override this._PhysicsProcess(delta) =
    if Active then
      if this.IsOnFloor() || this.IsOnWall() then
        YSpeed <- 0.0f
        speed <- 0.0f
      else
        YSpeed <- YSpeed - gravity
        speed <- this.InitialSpeed * delta
        let velocity = this.GlobalTransform.basis.x.Normalized() * speed
//        GD.Print(velocity.y + YSpeed)
        this.MoveAndSlide(
          Vector3(velocity.x, velocity.y + YSpeed, velocity.z),
          System.Nullable<Vector3>Vector3.Up,
          true, 4, Mathf.Pi/4f, false) |> ignore
