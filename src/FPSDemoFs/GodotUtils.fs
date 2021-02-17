module GodotUtils

open Godot

type Node with
  member this.getNode<'a when 'a :> Node> path =
    lazy (this.GetNode(new NodePath(path)) :?> 'a)
//  member this.emitSignal signal ([<System.ParamArray>] xs) =
//    this.EmitSignal(signal, xs)