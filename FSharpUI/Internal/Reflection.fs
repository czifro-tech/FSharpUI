namespace FSharpUI.Internal.Reflection

  open System
  open System.Reflection
  open FSharpUI.Internal.Events

  [<AutoOpen>]
  module internal ReflectionUtil =

    let getType (o:obj) =
      o.GetType()

    let inline getInstance<'a> types args =
      let t = typeof<'a>
      let ti = t.GetTypeInfo()
      let ctor = ti.GetConstructor(types)
      ctor.Invoke(args) :?> 'a

    let getProperty (name:string) (o:obj) =
      let t = getType o
      t.GetRuntimeProperty(name)

    let getPropertyValue (name:string) (o:obj) =
      let p = getProperty name o
      p.GetValue(o,null)

    let setPropertyValue (name:string) (o:obj) (v:obj) =
      let p = getProperty name o
      p.SetValue(o,v,null)
      o

    let getMethod (name:string) (o:obj) (paramTypes:Type[])=
      let t = getType o
      t.GetRuntimeMethod(name,paramTypes)

    let getEvent (name:string) (o:obj) =
      let t = getType o
      t.GetRuntimeEvent(name)

    let inline addEvent<'b when 'b :> EventArgs> (name:string) (o:obj) (onEvent:IOnEvent<'b>) =
      let e = getEvent name o
      e.AddEventHandler(o,onEvent)
      o

    let isUnitOrNone v =
      try
        (getType v) |> ignore
        false
      with
      | _ -> true

    let tryTupleToArray t =
      if isUnitOrNone t then None
      elif FSharp.Reflection.FSharpType.IsTuple(t.GetType()) then
        Some (FSharp.Reflection.FSharpValue.GetTupleFields t)
      else None