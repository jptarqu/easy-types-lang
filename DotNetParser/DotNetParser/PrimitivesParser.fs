namespace DotNetParser

module PrimitivesParser = 
    open PrimitivesParserTypes 
    open Common
    open System.Collections.Generic

    let buildPropLine(line: string, lineNum: int, linesParsed: LinkedList<PrimitiveSyntaxLineType>): unit  = 
        if (line.Length > 0) then
            let words = getNextWords(line, ' ')
            if (words.Length > 0) then
                let primitiveWordInfo = words.[0]
                let nameElement: PrimitiveNameElement = {
                    name =  primitiveWordInfo.word;
                    position = { startPos= 0; endPos= primitiveWordInfo.endIndex }
                }
                let mutable baseTypeElement: BasePrimitiveTypeElement option = None
                let mutable baseArguments: BasePrimtiveArgElement seq = Seq.empty
                if (words.Length > 1) then
                    let baseTypeWordInfo = words.[1]
                    baseTypeElement <- Some {
                        name= baseTypeWordInfo.word;
                        position= {
                            startPos = baseTypeWordInfo.startIndex;
                            endPos = baseTypeWordInfo.endIndex
                        }
                    }
                    if (words.Length > 2) then
                        let argsWords = words |> Array.skip(2)
                        baseArguments <-
                            argsWords 
                            |> Seq.map (
                                fun argWord -> 
                                    {
                                        value= argWord.word;
                                        position= {
                                                    startPos = argWord.startIndex;
                                                    endPos = argWord.endIndex
                                        }
                                    })
                             
                linesParsed.AddLast(
                    PrimitiveLine {
                        lineNum= lineNum;
                        nameElement = nameElement;
                        primitiveBaseTypeElement= baseTypeElement;
                        baseArguments = baseArguments
                    }
                ) |> ignore
            

        
    let parsePrimtivesLines(lines: LinkedList<string>): ParsePrimtivesFileInfo = 

        let linesParsed: LinkedList<PrimitiveSyntaxLineType> = LinkedList<PrimitiveSyntaxLineType>()
        let customPrimitives: LinkedList<CustomPrimitiveInfo> = LinkedList<CustomPrimitiveInfo>()
        lines
        |> Seq.iteri (fun lineIdx line -> 
            if (line.Length > 0 && not(line.StartsWith(" "))) then 
                buildPropLine(line, lineIdx + 2, linesParsed)
            )

            
        linesParsed
        |> Seq.iteri (fun lineIdx line -> 
            match line with
            | PrimitiveLine l -> 
                let currType = {
                            name = l.nameElement.name ;
                            baseType =  defaultArg (l.primitiveBaseTypeElement |> Option.map (fun o -> o.name)) "";
                            baseArgs = l.baseArguments |> Seq.map (fun a -> a.value) |> Seq.toArray
                        }
                customPrimitives.AddLast(currType) |> ignore
        )
            
        
        { linesParsed =linesParsed; customPrimitives = customPrimitives}
        
    