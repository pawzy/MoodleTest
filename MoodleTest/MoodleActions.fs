namespace MoodleTest

open System
open OpenQA.Selenium
open OpenQA.Selenium.Remote
open OpenQA.Selenium.Chrome
open NModel
open NModel.Conformance
open NModel.Terms

open MoodleModel


type Moodle () =
    let MOODLE_SITE = "http://localhost:8081/moodle/login/index.php"
    let MOODLE_USERNAME = "tudeng"
    let MOODLE_PASSWORD_CORRECT = "AjutineParool1#"
    let MOODLE_PASSWORD_INCORRECT = "ValeParool1#"
    //let MOODLE_PASSWORD_INCORRECT = "pw"
    let MOODLE_ID = Map.empty.Add("username", "username")
                             .Add("password", "password")
                             .Add("loginButton", "loginbtn")
                             .Add("itemVisibleWhenLoginSuccess", "nav-notification-popover-container")
    let options = ChromeOptions()
    
    static member val driver = null with get, set

    member this.Init () =
        options.AddAdditionalCapability(CapabilityType.Version, "latest", true)
        options.AddAdditionalCapability(CapabilityType.Platform, "WIN8", true);
        options.AddAdditionalCapability("key", "key", true);
        options.AddAdditionalCapability("secret", "secret", true);
        options.AddAdditionalCapability("name", "katsetus", true);
        Moodle.driver <- new ChromeDriver(options)
        this.OpenMoodle() 

    member this.OpenMoodle () =
        Moodle.driver.Navigate().GoToUrl(MOODLE_SITE)
        
    member this.Login (t:CompoundTerm) = 
        let usernameField = Moodle.driver.FindElementById(MOODLE_ID.Item("username"))
        let passwordField = Moodle.driver.FindElementById(MOODLE_ID.Item("password"))
        let loginButton = Moodle.driver.FindElementById(MOODLE_ID.Item("loginButton"))

        //Moodle.driver.FindElementByLinkText("Log in").Click()
        usernameField.SendKeys(MOODLE_USERNAME)

        if (string((t).[1]) = "Password(\"Correct\")") then 
            passwordField.SendKeys(MOODLE_PASSWORD_CORRECT)
        else 
            passwordField.SendKeys(MOODLE_PASSWORD_INCORRECT)        
        
        loginButton.Submit()

        let visibleElementWhenLoginSuccess = Moodle.driver.FindElementById(MOODLE_ID.Item("itemVisibleWhenLoginSuccess"))

        if visibleElementWhenLoginSuccess.Displayed then
            Action.Create("login_finish", (t).[0], LoginStatus.Success) :> CompoundTerm
        else 
            Action.Create("login_finish", (t).[0], LoginStatus.Failure) :> CompoundTerm
        
        //Moodle.driver.FindElementById("username").SendKeys("tudeng")
        //Moodle.driver.FindElementById("password").SendKeys("AjutineParool1#")
        //Moodle.driver.FindElementById("loginbtn").Submit()
        

    member this.GuestLogin () = 
        Moodle.driver.FindElementByLinkText("Log in").Click()
        Moodle.driver.FindElementById("guestlogin").Click()

    member this.Search() = 
        let searchBox = Moodle.driver.FindElementById("searchform_search")
        searchBox.SendKeys("Testkursus")
        searchBox.SendKeys(Keys.Enter)








    //static member public OpenBrowser () : ChromeDriver = 
    //    ()
   

    
    //static member public CorrectLogin (driver : ChromeDriver) =
    //    if driver = null then null
    //    else driver.Navigate().GoToUrl(MOODLE_SITe) driver
            

    //interface IStepper with 
    //    member this.DoAction(t:CompoundTerm) = 
    //        match t.FunctionSymbol.ToString() with 
    //        | "initialize" -> 
    //            Moodle.driver <- Moodle.OpenBrowser() 
    //            //term v6imaldab tagasi saata andmeid mudelisse
    //            null
    //        | "login" -> 
    //            Moodle.driver.Navigate().GoToUrl(MOODLE_SITE)
    //            Moodle.driver.FindElementByLinkText("Log in").Click()
    //            Moodle.driver.FindElementById("username").SendKeys("tudeng")
    //            Moodle.driver.FindElementById("password").SendKeys("AjutineParool1#")
    //            Moodle.driver.FindElementById("loginbtn").Submit()
    //            //Moodle.Driver.FindElementById("x")
           
    //            null
    //        | s -> failwith (sprintf "Tundmatu toiming %s" s)
    //    member this.Reset() = 
    //        Moodle.driver.Dispose()
    //        Moodle.driver <- Moodle.OpenBrowser() 
    //        ()
    

    //member this.CorrectLogin () =
    //    Moodle.driver.Navigate.GoToUrl(Moodle_SITE)


    //static member Create () = 
    //    new Moodle() 


        //let driver = Moodle.OpenBrowser



        //driver.Navigate().GoToUrl("http://localhost:8081/moodle/")
        //driver.FindElementByLinkText("Log in").Click()
        //driver.FindElementById("username").SendKeys("tudeng")
        //driver.FindElementById("password").SendKeys("AjutineParool1#")
        //driver.FindElementById("loginbtn").Submit()

        //driver



