namespace DotNetParser

module CustomRelationParserTypes = 
    open Common
    open System.Collections.Generic
    
    type Association = {
        toType: string
        cardinality: string
        fromProp: string
        toProp: string
    }
    type CustomRelationInfo = {
        name: string;
        props: LinkedList<Association>
    }

    type RelationNameElement= {
        typeName: string;
        position: PositionInfo
    }

    type RelationNameLine= {
        lineNum: int;
        nameElement: RelationNameElement option
    }

    type ToNameElement= {
        name: string;
        position: PositionInfo
    }
    type CardinalityElement= {
        value: string;
        position: PositionInfo
    }
    type PropNameElement= {
        name: string;
        position: PositionInfo
    }
    type AssociationLine= {
        lineNum: int;
        toElement: ToNameElement;
        cardinalityElement: CardinalityElement option
        fromPropElement: PropNameElement option
        toPropElement: PropNameElement option
    }
    type SyntaxLineType = 
        | AssociationLine of AssociationLine
        | RelationNameLine of RelationNameLine
    
    type ParseTypesFileInfo = {
        linesParsed: SyntaxLineType seq
        customLookups: CustomRelationInfo seq
    }
