namespace DotNetParser

module CustomLookupsParserTypes = 
    open Common
    open System.Collections.Generic
    
    type IdLabelPair = {
        id: string;
        label: string;
    }
    type CustomLookupInfo = {
        name: string;
        props: LinkedList<IdLabelPair>
    }

    type LookupNameElement= {
        typeName: string;
        position: PositionInfo
    }

    type LookupNameLine= {
        lineNum: int;
        nameElement: LookupNameElement option
    }

    type IdLabelPairNameElement= {
        name: string;
        position: PositionInfo
    }
    type IdLabelPairValueElement= {
        value: string;
        position: PositionInfo
    }
    type IdLabelPairLine= {
        lineNum: int;
        nameElement: IdLabelPairNameElement;
        valueElement: IdLabelPairValueElement option
    }
    type SyntaxLineType = 
        | IdLabelPairLine of IdLabelPairLine
        | LookupNameLine of LookupNameLine
    
    type ParseTypesFileInfo = {
        linesParsed: SyntaxLineType seq
        customLookups: CustomLookupInfo seq
    }
