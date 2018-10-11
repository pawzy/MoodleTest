#if INTERACTIVE
#r @"..\..\bin\NModel.dll"
#r @"..\..\bin\NModel.Visualization.dll"
#r @"bin\Debug\MoodleModel.dll"
#endif

let mp = MoodleModel.Contract.Create() 
NModel.Visualization.Interactive.Run(mp)


