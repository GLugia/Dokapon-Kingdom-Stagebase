namespace CharaReader.data
{
	/// <summary>
	/// A list of values with pointers labeling the (S)tart positions
	/// </summary>
	/// <typeparam name="T"></typeparam>
	//public class List_S<T> : List_Base<T> { }
	/// <summary>
	/// A list of values with pointers labeling the (S)tart and (E)nd positions
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class List_SE<T> : List_Base<T> { }
	/// <summary>
	/// A list of values with pointers labeling the (S)tart and End positions. However, the offset is set to the End position after reading.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class List_SP<T> : List_Base<T> { }
	/// <summary>
	/// A list of values with pointers labeling the (E)nd and (P)added positions
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class List_EP<T> : List_Base<T> { }
	/// <summary>
	/// A list of values with pointers labeling the (C)ount and (E)nd positions
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class List_CE<T> : List_Base<T> { }
	/// <summary>
	/// A list of values with a single pointer labeling the (C)ount to read
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class List_C<T> : List_Base<T> { }
	/// <summary>
	/// A list of values with a single pointer labeling the (E)nd position
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class List_E<T> : List_Base<T> { }
	/// <summary>
	/// A list of values with that end in their defined MaxValue constant. ie <see cref="int.MaxValue"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class List_Max_Terminated<T> : List_Base<T> { }
}
