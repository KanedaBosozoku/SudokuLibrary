namespace SudokuLib
 open System.Globalization
 open System
 open System.Collections.Generic
 open System.Collections
 open System.Collections.Generic
 module IndexMapping=




    //map index 0..80 to row number 0..8
    let indexToRowNumber x =
        x / 9

    //map index 0..80 to column number 0..8
    let indexToColumnNumber x =
        x % 9

    //map index 0..80 to block number 0..8
    let indexToBlockNumber x =
        match x > 80 with
        | false ->
            (x / 3) % 3 + (x / 27 * 3)
        | _ -> raise(System.IndexOutOfRangeException(sprintf "IndexToBlockNumber error: %d out of range" x))


    /// <summary>convert an index (0..80) to a block number (0..8).  The 9X9 board is divided to 3x3 blocks.</summary>
    /// <param name="index">Board index</param>
    /// <returns>A block number<returns>
    let indexToBlockIndex index =
        match index > 8 with
        | false ->
            (index / 3 * 9) 
        | _ -> raise(System.IndexOutOfRangeException(sprintf "indexToBlockIndex error: %d out of range" index))

    //first cell index of a gicen block 0..8
    let blockOffset index =
        match index>= 0 && index < 9 with
        | true ->
            index * 3 + index / 3 * 18
        | _ -> raise(System.InvalidOperationException())

    let blockMapper blk x =
        ((blockOffset blk) + (indexToBlockIndex (x % 9)) + (x % 3)) 
    

    let rowMapper row x =
        row * 9 + x

    let columnMapper column x =
        column % 9 + x * 9        


    let blockIndexSeq block =
        seq{0..8}
        |> Seq.map (fun x -> blockMapper block x)


    let rowIndexSeq row =
        seq{0..8}
        |> Seq.map (fun x -> rowMapper row x)


    let columnIndexSeq column =
        seq{0..8}
        |> Seq.map (fun x -> columnMapper column x)



    let squaresAffected pos =
        (indexToRowNumber pos |> rowIndexSeq, indexToColumnNumber pos |> columnIndexSeq)
        ||> Seq.append
        |> Seq.append (indexToBlockNumber pos |> blockIndexSeq)

    let printBlock nums =
        nums
        |> Seq.chunkBySize 3
        |> Seq.iter (fun x -> printfn "%A" x)