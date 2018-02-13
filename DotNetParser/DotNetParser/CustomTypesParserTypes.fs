namespace DotNetParser

module CustomTypesParserTypes = 
    open Common
    open System.Collections.Generic
    
    type Prop= {
        name: string;
        propTypeName: string;
    }
    type CustomType= {
        name: string;
        props: LinkedList<Prop>
    }

    type TypeNameElement= {
        typeName: string;
        position: PositionInfo
    }

    type TypeNameLine= {
        lineNum: int;
        nameElement: TypeNameElement option
    }

    type PropNameElement= {
        name: string;
        position: PositionInfo
    }
    type PropTypeElement= {
        name: string;
        position: PositionInfo
    }
    type PropLine= {
        lineNum: int;
        nameElement: PropNameElement;
        propTypeElement: PropTypeElement option
    }
    type SyntaxLineType = 
        | PropLine of PropLine
        | TypeNameLine of TypeNameLine
    
    type ParseTypesFileInfo = {
        linesParsed: SyntaxLineType seq
        customTypes: CustomType seq
    }
