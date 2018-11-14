open System
open System.Diagnostics
open System.IO

(* This example works as-is on the Mac *)

(* WINDOWS: This example should work on Windows with slight modifications:
   1. You will need to find equivalents for "afplay" and "say".
   2. On Windows, the System.Media namespace has tons of great
      media players (e.g., SoundPlayer).  See Stack Overflow
      for tips.  Sadly, System.Media does not exist in .NETCore,
      which is the cross-platform framework we are using. See
      me if you want help using the .NET standard for Windows.
   3. System.Speech.Synthesis also has lots of cool functionality.

   LINUX: This example should also work on Linux, but you may need to install
   some additional binaries.  We can probably do this with Mary's
   help for lab machines, so please ask me if you want to do this.
   1. The "play" command is the Linux equivalent of "afplay".
   2. The "espeak" command is the Linux equivalent of "say."
*)

(* Additional resources:

   The .NET platform has many APIs available in its library.  You can use the
   .NET API Browser to find a great deal of functionality:

   https://docs.microsoft.com/en-us/dotnet/api/index?view=netcore-2.1

   You should set the product to ".NET Core 2.1" when searching.

   The .NET platform also has an extensive collection of third-party
   libraries that you can download using NuGet, the .NET package
   manager.  Be aware that not all of these APIs are portable or
   compatible with .NET Core 2.1.  However, the most popular libraries
   are moving in that direction.

   The code below uses only .NET Core cross-platform APIs, although
   the precise commands used (e.g., afplay, say, and open) will
   vary depending on your platform.
*)

(* PLAYING SOUND FILES *)
let initWAVProcess(wavpath: string) : Process =
    // "afplay" only works on the Mac
    let info = new ProcessStartInfo (
                 FileName = "/bin/bash",
                 Arguments = "-c \"afplay " + wavpath + "\"",
                 RedirectStandardOutput = true,
                 UseShellExecute = false,
                 CreateNoWindow = true
               )
    let p = new Process()
    p.StartInfo <- info
    p

let playWAV(wavpath: string) : unit =
    let p = initWAVProcess wavpath
    if p.Start() then
        let result = p.StandardOutput.ReadToEnd()
        p.WaitForExit()
    else
        printfn "Could not start afplay."
        exit 1    

(* SPEAKING *)
let initSpeechProcess(words: string): Process =
    // "say" only works on the Mac
    let info = new ProcessStartInfo (
                 FileName = "/bin/bash",
                 Arguments = "-c \"say " + words + "\"",
                 RedirectStandardOutput = true,
                 UseShellExecute = false,
                 CreateNoWindow = true
               )
    let p = new Process()
    p.StartInfo <- info
    p

let speakWords(words: string) : unit =
    let p = initSpeechProcess words
    if p.Start() then
        let result = p.StandardOutput.ReadToEnd()
        p.WaitForExit()
    else
        printfn "Could not start speech synthesizer."
        exit 1

(* DRAWING GRAPHICS FILES *)
let circle x y radius fill =
    "<circle cx='" + x.ToString() +
    "' cy='" + y.ToString()  +
    "' r='" + radius.ToString() +
    "' style='fill: " + fill + ";'>" +
    "</circle>"

let polyline xys width color =
    let rec pl xys : string list =
       match xys with
       | (x,y)::xys' -> x.ToString() + " " + y.ToString() :: (pl xys')
       | [] -> []
    "<polyline points='" +
    String.Join(",", List.rev (pl xys)) + "' " +
    "stroke-width='" + width.ToString() + "' " +
    "stroke='" + color + "' " +
    "style='fill: none;' " +
    "/>"

let svgDraw guts =
    // The triple-double quote thing is called a "heredoc".
    // It lets us write long string literals that include
    // newlines, etc.
    let header = """<!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.0//EN" 
 "http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd">

<svg xmlns="http://www.w3.org/2000/svg" 
 xmlns:xlink="http://www.w3.org/1999/xlink" 
 width='300px' height='300px'>"""

    let footer = """</svg>"""
    header + guts + footer

let initGraphicsProcess(svgpath: string): Process =
    // "open" only works on the Mac
    let info = new ProcessStartInfo (
                 FileName = "/bin/bash",
                 Arguments = "-c \"open " + svgpath + "\"",
                 RedirectStandardOutput = true,
                 UseShellExecute = false,
                 CreateNoWindow = true
               )
    let p = new Process()
    p.StartInfo <- info
    p

let displaySVG(svgpath: string) : unit =
    let p = initGraphicsProcess svgpath
    if p.Start() then
        let result = p.StandardOutput.ReadToEnd()
        p.WaitForExit()
    else
        printfn "Could not open SVG."
        exit 1

[<EntryPoint>]
let main argv =
    printfn "Playing a sound..."
    playWAV "wonderful.wav"

    printfn "Saying something..."
    speakWords "Programming really is wonderful!"
    let svg = svgDraw (
                (circle 10 10 5 "red") +
                (circle 30 10 5 "blue") +
                (polyline [(10,20);(15,30);(25,30);(30,20)] 4 "green")
              )

    printfn "Writing an SVG to a file and opening with your web browser..."
    File.WriteAllText("output.svg", svg)
    displaySVG "output.svg"
    Threading.Thread.Sleep(5000)  // wait for the web browser to start up
    File.Delete "output.svg" // cleanup
    0
