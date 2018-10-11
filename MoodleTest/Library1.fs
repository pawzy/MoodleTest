namespace MoodleTest

open System
open OpenQA.Selenium
open OpenQA.Selenium.Remote
open OpenQA.Selenium.Chrome
open NModel
open NModel.Conformance
open NModel.Terms
open System.Threading

type Moodle () =
    let MOODLE_SITE = "http://localhost:8081/moodle/"
    let MOODLE_USERNAME = "tudeng"
    let MOODLE_PASSWORD_CORRECT = "AjutineParool1#"
    let MOODLE_PASSWORD_INCORRECT = "pw"
    let MOODLE_COURSE_SEARCHED = "Testkursus 1"

    static member val driver = null with get, set

    static member public OpenBrowser () : ChromeDriver = 
        let options = ChromeOptions() 
        options.AddAdditionalCapability(CapabilityType.Version, "latest", true)
        options.AddAdditionalCapability(CapabilityType.Platform, "WIN8", true);
        options.AddAdditionalCapability("key", "key", true);
        options.AddAdditionalCapability("secret", "secret", true);
        options.AddAdditionalCapability("name", "katsetus", true);
        let driver = new ChromeDriver(options)
        
        driver
            

    interface IStepper with 
        member this.DoAction(t:CompoundTerm) = 
            match t.FunctionSymbol.ToString() with 
            | "initialize" -> 
                Moodle.driver <- Moodle.OpenBrowser() 
                //term v6imaldab tagasi saata andmeid mudelisse
                Moodle.driver.Navigate().GoToUrl(MOODLE_SITE)
                //Moodle.driver.ExecuteScript("window.resizeTo(screen.width, screen.height)") |> ignore
                //Moodle.driver.ExecuteScript("console.log(123123)") |> ignore
                null
            | "login" -> 
                Moodle.driver.FindElementByLinkText("Log in").Click()
                Moodle.driver.FindElementById("username").SendKeys("tudeng")
                Moodle.driver.FindElementById("password").SendKeys("AjutineParool1#")
                Moodle.driver.FindElementById("loginbtn").Submit()
                //Moodle.Driver.FindElementById("x")
                //Moodle.driver.FindElementByName("Search")
                
                //let searchBox = Moodle.driver.FindElementByXPath("//form/input[contains(@id,'id_q_5bb')]") //navbar search
                
                let searchBox = Moodle.driver.FindElementById("searchform_search")

                System.Threading.Thread.Sleep(20000)
                searchBox.SendKeys("Testkursus")
                searchBox.SendKeys(Keys.Enter)
                System.Threading.Thread.Sleep(200)
                //Moodle.driver
           
                null
            | s -> failwith (sprintf "Tundmatu toiming %s" s)
        member this.Reset() = 
            System.Threading.Thread.Sleep(200)

            Moodle.driver.Dispose()
            Moodle.driver <- Moodle.OpenBrowser() 
            ()


    static member Create () = 
        new Moodle() 


        //let driver = Moodle.OpenBrowser



        //driver.Navigate().GoToUrl("http://localhost:8081/moodle/")
        //driver.FindElementByLinkText("Log in").Click()
        //driver.FindElementById("username").SendKeys("tudeng")
        //driver.FindElementById("password").SendKeys("AjutineParool1#")
        //driver.FindElementById("loginbtn").Submit()

        //driver



