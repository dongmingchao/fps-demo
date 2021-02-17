namespace FPSDemo

open Godot
open GodotUtils
open FPSDemo

//type rotateZ = delegate of (float) -> unit

type FPSCamera() as this =
  inherit Spatial()
  let mouse_sensitivity = 0.1f
  let speed = 10.0f
  let acceleration = 5.0f
  let mutable direction = Vector3()
  let mutable velocity = Vector3()

  let processMoveMent delta =
    direction <- direction.Normalized()
    velocity <- direction * speed
    let hvel = velocity.LinearInterpolate(velocity, acceleration * delta)
    velocity <- Vector3(hvel.x, velocity.y, hvel.z)
    this.EmitSignal("Move", velocity, delta)
  override this._Ready() =
     GD.Print("Hello from F#!")
     
  override this._Input(event) =
    if Input.IsActionJustPressed("ui_cancel") then
      if Input.GetMouseMode() = Input.MouseMode.Captured
      then Input.SetMouseMode(Input.MouseMode.Visible)
      else Input.SetMouseMode(Input.MouseMode.Captured)
    if event :? InputEventMouseMotion then
      let e = event :?> InputEventMouseMotion
      Mathf.Deg2Rad(-e.Relative.x * mouse_sensitivity)
      |> fun d -> this.EmitSignal("RotateY", d)
      Mathf.Deg2Rad(e.Relative.y * mouse_sensitivity)
      |> fun d ->
        let finalX = d + this.Rotation.x
        if finalX < Mathf.Deg2Rad(-90.0f) || finalX > Mathf.Deg2Rad(90.0f)
        then 0.0f
        else
          this.EmitSignal("RotateZ", -d)
          -d
      |> this.RotateZ
      
  override this._PhysicsProcess(delta) =
    let camx = this.GlobalTransform.basis
    
    (* Walking *)
    direction <- Vector3()
    if Input.IsKeyPressed(int KeyList.W)
    then direction <- direction + camx.x
    if Input.IsKeyPressed(int KeyList.S)
    then direction <- direction - camx.x
    if Input.IsKeyPressed(int KeyList.A)
    then direction <- direction - camx.z
    if Input.IsKeyPressed(int KeyList.D)
    then direction <- direction + camx.z
    processMoveMent delta

type Player() as this =
  inherit KinematicBody()
  let camera = this.getNode<Spatial>("Camera")
  let gun = this.getNode<MeshInstance>("Mesh/Gun")
  let body = this.getNode<CollisionShape>("Mesh")
  let gravity = 69.8f
  let jumpForce = 20.0f
  let mutable ySpeed = 0.0f
  let mutable velocity = Vector3()
  let mutable willJump = false

  let bullet = ResourceLoader.Load("res://Bullet.tscn") :?> PackedScene
  member this.onCameraRotateY(d: float32) =
    this.RotateObjectLocal(Vector3(0.0f, 1.0f, 0.0f), d)
  member this.onCameraRotateZ(d: float32) =
    body.Value.RotateZ(d)
  member this.onMove(d: Vector3, delta: float32) =
    let mutable yMove = 0.0f
    if willJump then
      velocity <- Vector3.Up * jumpForce
      willJump <- false
    elif this.IsOnFloor() then
      ySpeed <- 0.0f
      velocity <- Vector3()
    else
      ySpeed <- ySpeed - gravity * delta
      yMove <- ySpeed + yMove
    let finalVelocity = Vector3(d.x, yMove, d.z) + velocity
    if finalVelocity.x <> 0.0f || finalVelocity.y <> 0.0f || finalVelocity.z <> 0.0f then
      this.MoveAndSlide(finalVelocity, System.Nullable<Vector3> Vector3.Up, true, 4, Mathf.Pi/4f, false) |> ignore
  override this._Ready() =
    let err = camera.Value.Connect("RotateY", this, "onCameraRotateY")
    let err = camera.Value.Connect("RotateZ", this, "onCameraRotateZ")
    let err = camera.Value.Connect("Move", this, "onMove")
    ()
  override this._PhysicsProcess(delta) =
    if Input.IsActionJustPressed("ui_select") && this.IsOnFloor() then
      willJump <- true
    if Input.IsActionJustPressed("fire") then
      let b = bullet.Instance() :?> Bullet.Class
//      b.Translate(Vector3(1.0f, 0.0f, 0.0f))
      b.Scale <- Vector3(0.2f, 0.2f, 0.2f)
      b.InitialSpeed <- 3000f
      gun.Value.AddChild(b)
//      b.Active <- true
      ()