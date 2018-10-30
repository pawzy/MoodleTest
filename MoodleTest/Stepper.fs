namespace MoodleTest

open NModel.Conformance
open NModel.Terms
open System
open System.IO

type Stepper () = 
    let moodle = new Moodle ()
    do moodle.Init ()

    static member Create() = 
        new Stepper ()

    interface IStepper with 
        member this.DoAction (t : CompoundTerm) = 
            match t.FunctionSymbol.ToString() with
            | "login_start" -> moodle.Login (t)
            | "logout_start" -> moodle.Logout (t)
            | "guestLogin" -> null
            | "search_start" -> moodle.Search ()
            | s -> failwith (sprintf "Tundmatu toiming, %s" s)
        member this.Reset() = 
            System.Threading.Thread.Sleep(2000)
            Moodle.driver.Quit()
            moodle.Init ()


