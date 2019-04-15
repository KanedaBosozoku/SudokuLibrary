module SudokuLib.RandomAccessList

type Tree<'a> =
   | Leaf of 'a
   | Node of 'a * 'a Tree * 'a Tree

type RList<'a> =
   | Nil
   | Cons of (int * Tree<'a>) * RList<'a>
   
type tree<'a> = Tree<'a>
type rlist<'a> = RList<'a>

module RList =
   val empty : 'a rlist
   val cons : 'a -> 'a rlist -> 'a rlist
   val singleton : 'a -> 'a rlist
   val length : 'a rlist-> int
   val head : 'a rlist -> 'a
   val tail : 'a rlist -> 'a rlist
   val nth : int -> 'a rlist -> 'a
   val update : int -> 'a -> 'a rlist -> 'a rlist
   val fromList : 'a list -> 'a rlist
   val toList : 'a rlist -> 'a list