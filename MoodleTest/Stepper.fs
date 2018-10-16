namespace MoodleTest

open NModel.Conformance
open NModel.Terms
open System
open System.IO

type Stepper () = 
    let moodle = new Moodle()
    do moodle.Init ()

    static member Create() = 
        new Stepper ()

    interface IStepper with 
        member this.DoAction (t:CompoundTerm) = 
            match t.FunctionSymbol.ToString() with
            | "correctLogin" -> null
            | "incorrectLogin" -> null
            | "guestLogin" -> null
            | "searchCourse" -> null
            | s -> failwith (sprintf "Tundmatu toiming, %s" s)
        member this.Reset() = 
            () 



