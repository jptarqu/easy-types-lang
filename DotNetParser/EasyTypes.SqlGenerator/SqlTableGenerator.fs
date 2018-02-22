namespace EasyTypes.SqlGenerator

module SqlTableGenerator =
    open DotNetParser.PrimitivesParserTypes
    open DotNetParser.SemanticTypes
    open SqlCommon

    let private buildColumn (p:TypeProperty): string =
        let colCode = p.name + " " + convertToSqlType(p.propType.baseType) 
        if isPrimaryKey p then
            colCode + " PRIMARY KEY identity"
        else 
            colCode

    let buildTable (customType: CustomType ) : string =
        let cols = customType.props |> Seq.map buildColumn 
        let header = "CREATE TABLE [" + (customType.name) + "]"
        "\n" + header + "\n(\n" + (String.concat ",\n" cols) + "\n)\n"