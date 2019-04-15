namespace SudokuLib
open FSharp.Data
//open FSharpx.Collections
//open SudokuLib.RandomAccessList
open System.Collections.Generic

type XmlPuzzle = XmlProvider<FileTypes.xmlSample> 

module BoardReadWrite =
    open System
    open System.IO
    open System.Xml
    open System.Xml.Linq
    open System.Xml.Serialization

    open FSharp


    open FSharp.Data.Runtime
    open SquareType
    open FileTypes

    let ReadCsv(fname : string) =
        seq {
                use data = CsvFile.Load(fname, hasHeaders=false)
                for row in data.Rows do 
                for col in row.Columns do
                match col with
                | "0" -> yield EmptyValue
                | symbol when not (String.IsNullOrEmpty(symbol)) ->
                    yield FixedConstraint(Int32.Parse(symbol))
                | " " -> yield EmptyValue
                | err -> raise(InvalidDataException(sprintf "invalid symbol: %A" err ))
            }


    let readDocPart(doc : XmlPuzzle.Puzzle) =
        query{
            for rows in doc.XElement.Elements("board") do
                for row in rows.Elements() do
                    let rowIdx = Int32.Parse (row.Attribute("index").Value)
                    for cell in row.Elements() do
                        let colIdx = (Int32.Parse (cell.Attribute("index").Value))
                        let readOnly = (bool.Parse(cell.Attribute("readOnly").Value))
                        select 
                            (rowIdx * 9 + colIdx, readOnly, Int32.Parse cell.Value)
                        }

    let rec expander acc (index : list<int>) (squares :  list<int * bool  * int>) =
        match index, squares with
        | cnt::irest, (idx ,true,num)::rest when cnt = idx -> expander (Square.FixedConstraint(num)::acc) irest rest
        | cnt::irest, (idx ,false,num)::rest when cnt = idx -> expander (Square.Constraint(num)::acc) irest rest
        | _::irest, _ -> expander (Square.EmptyValue::acc) irest squares
        | [], _ -> List.rev acc

    let expandDoc size (squares : seq<int * bool * int>) =
        squares
        |> Seq.toList
        |> expander [] [0..(size - 1)] 

    let private readDoc(doc : XmlPuzzle.Puzzle) =
        readDocPart doc
        |> expandDoc 81

    let ReadXml(fname : string) =
        readDoc(XmlPuzzle.Load(fname))
        
    let ReadXmlString(puzzle : string) =
        readDoc(XmlPuzzle.Parse(puzzle))

    let ConvertBoardToRows(board : IEnumerable<ISquare>) =
        board|> Seq.chunkBySize 9
        |> Seq.mapi (fun ridx row ->
            {index=ridx; Col =
                row |> Array.mapi (fun colIdx sq ->
                    let a1, a2 = sq.TryGetConstraint()
                    let b1, b2 = sq.TryGetFixedConstraint()
                    if(a1) then
                        (colIdx, a2, false, true)
                    else if(b1) then
                        (colIdx, b2, true, true)
                    else
                        (0, 0, false, false))
                |> Array.choose (fun x ->
                    match x with
                    | idx, dat, false, true -> Some({index=idx; readOnly= false; data=dat})
                    | idx, dat, true, true -> Some({index=idx; readOnly= true; data=dat})
                    | _, _, _, false -> None
                    )})
        |> Seq.toArray
                


    let SquaresToRows(board : seq<Square>) =
        board|> Seq.chunkBySize 9
        |> Seq.mapi (fun ridx row ->
            {index=ridx; Col =
                row |> Array.mapi (fun colIdx sq ->
                    match sq with
                    | Constraint(x) -> (colIdx, x, false, true)
                    | FixedConstraint(x) -> (colIdx, x, true, true)
                    | _ ->    (0, 0, false, false))
                |> Array.choose (fun x ->
                    match x with
                    | idx, dat, false, true -> Some({index=idx; readOnly= false; data=dat})
                    | idx, dat, true, true -> Some({index=idx; readOnly= true; data=dat})
                    | _, _, _, false -> None
                    )})
        |> Seq.toArray



    let WritePuzzleXml((filename : string), (p : Row[]))  =
        let stream = new StreamWriter(filename)
        let settings = XmlWriterSettings()
        settings.Indent <- true
        let writer = XmlWriter.Create(stream, settings)
       
        let ns = XmlSerializerNamespaces();
        ns.Add("", "");

        let puzzle = {size=9; header={version=1}; board=p}

        let serializer = XmlSerializer(typeof<PuzzleXml>)
        serializer.Serialize(writer, puzzle, ns)
        writer.Dispose()
