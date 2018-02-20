namespace DotNetParser

module Common = 
    open System.Collections.Generic

    type PositionInfo = {
        startPos: int
        endPos: int
    }
    
    type WordInfo =  {
        word: string
        restOfLine: string
        startIndex: int
        endIndex: int
    }

    let getIndexAfterToken(str: string, token: char) = 
        let mutable idx = 0
        seq {
            for char in str do
                if (char <> token) then
                    yield idx
                idx <- idx + 1
            yield idx
        }
        |> Seq.head
    
    let getNextWord(line: string, wordBoundary: char): WordInfo =
        if line.Length > 0 then 
            let mutable startIndex = 0
            if (line.[0] = wordBoundary) then
                startIndex <- getIndexAfterToken(line, wordBoundary)
            let firstWordEndIdx = line.IndexOf(wordBoundary, startIndex)
            if (firstWordEndIdx > 1) then
                let word = line.Substring(startIndex, firstWordEndIdx - startIndex)
                let restOfLine = line.Substring(firstWordEndIdx)
                { word = word; restOfLine = restOfLine; startIndex = startIndex; endIndex = firstWordEndIdx }
            else
                let word = line.Substring(startIndex)
                let restOfLine = ""
                { word =  word; restOfLine = restOfLine; startIndex = startIndex; endIndex=  0 }
         else 
            { word =  ""; restOfLine = line; startIndex = 0; endIndex=  0 }
        
    let getNextWords(line: string, wordBoundary: char): WordInfo[] =
        let words = LinkedList<WordInfo>()
        let mutable currWord: WordInfo = getNextWord(line, wordBoundary)
        while (currWord.word <> "") do
            words.AddLast(currWord) |> ignore
            currWord <- getNextWord(currWord.restOfLine, wordBoundary)
        words |> Seq.toArray
    