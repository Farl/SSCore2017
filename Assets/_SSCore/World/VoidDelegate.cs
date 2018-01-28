using UnityEngine;

using System.Collections;
using System.Collections.Generic;


namespace SS
{
	public delegate void VoidDelegate();

	public class SortedVoidDelegate
	{
		SortedDictionary<int, List<VoidDelegateElement>> dictionary = new SortedDictionary<int, List<VoidDelegateElement>>();

		bool Add(VoidDelegate _callback, object _key, int _order)
		{
			bool result = false;
			List<VoidDelegateElement> currList;

			if (!dictionary.ContainsKey(_order))
			{
				currList = new List<VoidDelegateElement>();
				VoidDelegateElement ele = new VoidDelegateElement(_callback, _key, _order);
				currList.Add(ele);
				dictionary.Add(_order, currList);
			}
			else
			{
				currList = dictionary[_order];

				bool found = false;
				foreach (VoidDelegateElement ele in currList)
				{
					if (ele.callback == _callback && ele.key == _key)
					{
						// Already exist
						found = true;
					}
				}

				// TODO: check duplicate item in different order
				if (!found)
				{
					// Add
					VoidDelegateElement ele = new VoidDelegateElement(_callback, _key, _order);
					currList.Add(ele);
				}

			}
			return result;
		}

		bool Remove(VoidDelegate _callback, object _key)
		{
			foreach (KeyValuePair<int, List<VoidDelegateElement>> kvp in dictionary)
			{
				List<VoidDelegateElement> currList = kvp.Value;
				if (currList != null)
				{
					if (Remove(_callback, _key, kvp.Key))
					{
						return true;
					}
				}
			}
			return false;
		}

		bool Remove(VoidDelegate _callback, object _key, int _order)
		{
			bool result = false;
			if (dictionary.ContainsKey(_order))
			{
				List<VoidDelegateElement> currList = dictionary[_order];

				foreach (VoidDelegateElement ele in currList)
				{
					if (ele.callback == _callback && ele.key == _key)
					{
						currList.Remove(ele);
						result = true;
						break;
					}
				}
				if (currList.Count <= 0)
				{
					dictionary.Remove(_order);
				}
			}
			return result;
		}

		bool IsContain(VoidDelegate _callback, object _key, int _order)
		{
			if (dictionary.ContainsKey(_order))
			{
				foreach (VoidDelegateElement ele in dictionary[_order])
				{
					if (ele.callback == _callback && ele.key == _key)
					{
						return true;
					}
				}
			}
			return false;
		}
		
		public void Register(VoidDelegate _callback, object _key, int _order)
		{
			Add(_callback, _key, _order);
		}

		public void Unregister(VoidDelegate _callback, object _key, int _order)
		{
			Remove(_callback, _key);
		}

		public void Invoke()
		{
			List<int> removeKey = new List<int>();

			foreach (KeyValuePair<int, List<VoidDelegateElement>> kvp in dictionary)
			{
				List<VoidDelegateElement> currList = kvp.Value;
				if (currList != null)
				{
					List<VoidDelegateElement> removeList = new List<VoidDelegateElement>();

					foreach (VoidDelegateElement ele in currList)
					{
						// Check key(instance)
						if (!ele.Invoke())
						{
							removeList.Add(ele);
						}
					}
					
					foreach (VoidDelegateElement ele in removeList)
					{
						currList.Remove(ele);
					}

					if (currList.Count <= 0)
					{
						removeKey.Add(kvp.Key);
					}
				}
				else
				{
					removeKey.Add(kvp.Key);
				}
			}
			foreach (int k in removeKey)
			{
				dictionary.Remove(k);
			}
		}

		public void Refresh()
		{
			List<int> removeKey = new List<int>();

			foreach (KeyValuePair<int, List<VoidDelegateElement>> kvp in dictionary)
			{
				List<VoidDelegateElement> currList = kvp.Value;
				if (currList != null)
				{
					List<VoidDelegateElement> removeList = new List<VoidDelegateElement>();

					foreach (VoidDelegateElement ele in currList)
					{
						if (!ele.IsValid())
						{
							removeList.Add(ele);
						}
					}

					foreach (VoidDelegateElement ele in removeList)
					{
						currList.Remove(ele);
					}

					if (currList.Count <= 0)
					{
						removeKey.Add(kvp.Key);
					}
				}
				else
				{
					removeKey.Add(kvp.Key);
				}
			}
			foreach (int k in removeKey)
			{
				dictionary.Remove(k);
			}
		}
	}

	public class VoidDelegateElement
	{
		public int order;
		public object key;
		public VoidDelegate callback;
		
		public VoidDelegateElement(VoidDelegate _callback, object _key, int _order)
		{
			callback = _callback;
			key = _key;
			order = _order;
		}

		public bool IsValid()
		{
			if (callback != null && key != null)
			{
				if (key.GetType().IsSubclassOf(typeof(UnityEngine.Object)))
				{
					UnityEngine.Object obj = key as UnityEngine.Object;
					if (obj != null)
					{
						return true;
					}
				}
				else
				{
					return true;
				}
			}
			return false;
		}
		
		public bool Invoke()
		{
			if (IsValid())
			{
				callback();
				return true;
			}
			return false;
		}
	}
}