namespace EasyTypes.SqlGenerator

module SqlTableGenerator =
    open DotNetParser.PrimitivesParserTypes
    open DotNetParser.SemanticTypes

    let private convertToSqlType dataReqs =
        match dataReqs with
        | CommonDataRequirementsString dreq -> "varchar(" +  dreq.Size.ToString() + ")"
        | CommonDataRequirementsInt _ -> "int"
        | CommonDataRequirementsDecimal dreq -> "numeric(" +  dreq.Size.ToString() + ", " + dreq.Precision.ToString() + ")"
        | CommonDataRequirementsDate _ -> "Date"
        | CommonDataRequirementsDateTime _ -> "DateTime"
        | CommonDataRequirementsMoney _ -> "Money"
        | CommonDataRequirementsBinary _ -> "varbinary(max)"
        | _ -> "varchar(10)"
    let private isPrimaryKey  (p:TypeProperty) = 
        p.propType.name = "IntId"

    let private buildColumn (p:TypeProperty): string =
        let colCode = p.name + " " + convertToSqlType(p.propType.baseType) + " NOT NULL"
        if isPrimaryKey p then
            colCode + " PRIMARY KEY identity"
        else 
            colCode

    let buildTable (customType: CustomType ) : string =
        let cols = customType.props |> Seq.map buildColumn 
        let header = "CREATE TABLE [" + (customType.name) + "]"
        "\n" + header + "\n(\n" + (String.concat ",\n" cols) + "\n)\n"