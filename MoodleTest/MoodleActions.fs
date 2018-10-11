namespace MoodleTest

//open System
//open OpenQA.Selenium
//open OpenQA.Selenium.Remote
//open OpenQA.Selenium.Chrome
//open NModel
//open NModel.Conformance
//open NModel.Terms



//module MoodleActions =
//    type Moodle () =
//        let MOODLE_SITE = "http://localhost:8081/moodle/"
//        let MOODLE_USERNAME = "tudeng1"
//        let MOODLE_PASSWORD_CORRECT = "AjutineParool1#"
//        let MOODLE_PASSWORD_INCORRECT = "pw"
//        let options = ChromeOptions()
    
//        static member val driver = null with get, set


//        member this.Init () =
//            options.AddAdditionalCapability(CapabilityType.Version, "latest", true)
//            options.AddAdditionalCapability(CapabilityType.Platform, "WIN8", true);
//            options.AddAdditionalCapability("key", "key", true);
//            options.AddAdditionalCapability("secret", "secret", true);
//            options.AddAdditionalCapability("name", "katsetus", true);
//            Moodle.driver <- new ChromeDriver(options)
//            this.OpenMoodle() 

//        member this.OpenMoodle () =
//            Moodle.driver.Navigate().GoToUrl(MOODLE_SITE)
        
//        member this.CorrectLogin () = 
//            Moodle.driver.FindElementByLinkText("Log in").Click()
//            Moodle.driver.FindElementById("username").SendKeys("tudeng")
//            Moodle.driver.FindElementById("password").SendKeys("AjutineParool1#")
//            Moodle.driver.FindElementById("loginbtn").Submit()

//        member this.GuestLogin () = 
//            Moodle.driver.FindElementByLinkText("Log in").Click()
//            Moodle.driver.FindElementById("guestlogin").Click()


//open MoodleActions

//type Stepper () = 
//    let moodle = new Moodle ()
//    do moodle.Init ()

//    static member Create() = 
//        new Stepper ()

//    interface IStepper with 
//        member this.DoAction (t:CompoundTerm) = 
//            match t.FunctionSymbol.ToString() with
//            | "correctLogin" -> 
//                moodle.CorrectLogin () 
//                null
//            | "incorrectLogin" -> null
//            | "guestLogin" -> null
//            | "searchCourse" -> null
//            | s -> failwith (sprintf "Tundmatu toiming, %s" s)
//        member this.Reset() = 
//            ()








//    //static member public OpenBrowser () : ChromeDriver = 
//    //    ()
   

    
//    //static member public CorrectLogin (driver : ChromeDriver) =
//    //    if driver = null then null
//    //    else driver.Navigate().GoToUrl(MOODLE_SITe) driver
            

//    //interface IStepper with 
//    //    member this.DoAction(t:CompoundTerm) = 
//    //        match t.FunctionSymbol.ToString() with 
//    //        | "initialize" -> 
//    //            Moodle.driver <- Moodle.OpenBrowser() 
//    //            //term v6imaldab tagasi saata andmeid mudelisse
//    //            null
//    //        | "login" -> 
//    //            Moodle.driver.Navigate().GoToUrl(MOODLE_SITE)
//    //            Moodle.driver.FindElementByLinkText("Log in").Click()
//    //            Moodle.driver.FindElementById("username").SendKeys("tudeng")
//    //            Moodle.driver.FindElementById("password").SendKeys("AjutineParool1#")
//    //            Moodle.driver.FindElementById("loginbtn").Submit()
//    //            //Moodle.Driver.FindElementById("x")
           
//    //            null
//    //        | s -> failwith (sprintf "Tundmatu toiming %s" s)
//    //    member this.Reset() = 
//    //        Moodle.driver.Dispose()
//    //        Moodle.driver <- Moodle.OpenBrowser() 
//    //        ()
    

//    //member this.CorrectLogin () =
//    //    Moodle.driver.Navigate.GoToUrl(Moodle_SITE)


//    //static member Create () = 
//    //    new Moodle() 


//        //let driver = Moodle.OpenBrowser



//        //driver.Navigate().GoToUrl("http://localhost:8081/moodle/")
//        //driver.FindElementByLinkText("Log in").Click()
//        //driver.FindElementById("username").SendKeys("tudeng")
//        //driver.FindElementById("password").SendKeys("AjutineParool1#")
//        //driver.FindElementById("loginbtn").Submit()

//        //driver



