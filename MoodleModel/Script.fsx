#if INTERACTIVE
#r @"bin\Debug\NModel.dll"
#r @"bin\Debug\NModel.Visualization.dll"
#r @"bin\Debug\MoodleModel.dll"
#endif

let mp = MoodleModel.Factory.Create() 
NModel.Visualization.Interactive.Run(mp)


