using System.Text.RegularExpressions;

namespace Eliot.Repository
{
	/// <summary>
	/// Responsible for creating JsonObjects from a json string.
	/// </summary>
	[System.Obsolete] public static class JsonParser
	{
		/// <summary>
		/// Possible tokens that can be found in json string.
		/// </summary>
		private enum Token
		{
			LC = '{', RC = '}', LS = '[', RS = ']',
			COMA = ',', COLUMN = ':', QUOTE = '"', STRING
		}

		/// Json string parsed to the object.
		private static string _json;
		/// Index of currently read symbol in the string.
		private static int _curSymbol;
		/// Token type of the current symbol.
		private static Token _curToken = Token.LC;
		/// Current symbol.
		private static string _curChar = "";
		
		/// <summary>
		/// Return JsonObject from json string object.
		/// </summary>
		/// <returns></returns>
		private static JsonObject get_object()
		{
			var obj = new JsonObject();
			while(_curSymbol < _json.Length)
			{
				next();
				switch (_curToken)
				{
					case Token.RC:
						return obj;
					case Token.LC:
						return get_object();
					default:
						obj.Objects.Add(get_pair());
						break;
				}
			}

			return obj;
		}
		
		/// <summary>
		/// Return JsonObject from a pair of variable name and its value.
		/// </summary>
		/// <returns></returns>
		private static JsonObject get_pair()
		{
			var pair = new JsonObject();
			while(_curSymbol < _json.Length)
			{
				switch (_curToken)
				{
					case Token.QUOTE:
						pair.Name = get_string();
						break;
					case Token.STRING:
						pair.Name = get_string(_curChar);
						break;
					case Token.COLUMN:
						pair.Objects.Add(get_value(/*pair*/));
						return pair;
				}
				next();
			}

			return pair;
		}
		
		/// <summary>
		/// Return JsonObject that is a value, omitting its name.
		/// </summary>
		/// <returns></returns>
		private static JsonObject get_value(JsonObject parent = null)
		{
			var value = new JsonObject();
			while(_curSymbol < _json.Length)
			{
				next();
				switch (_curToken)
				{
					case Token.STRING:
						value.Objects.Add(get_string(_curChar));
						return value;
					case Token.QUOTE:
						value.Objects.Add(get_string());
						return value;
					case Token.LS:
						if (parent != null) parent.IsArray = true;
						value.Objects.Add(get_array());
						return value;
					default:
						value.Objects.Add(get_object());
						return value;
				}
			}

			return value;
		}
		
		/// <summary>
		/// Return JsonObject that is an array of values.
		/// </summary>
		/// <returns></returns>
		private static JsonObject get_array()
		{
			var arr = new JsonObject {IsArray = true};
			while(_curSymbol < _json.Length)
			{
				switch (_curToken)
				{
					case Token.RS:
						return arr;
					default:
						arr.Objects.Add(get_value());
						next();
						break;
				}
			}

			return arr;
		}
		
		/// <summary>
		/// Return JsonObject from a json string that is a single value.
		/// </summary>
		/// <returns></returns>
		private static string get_string(string initStr = "")
		{
			var str = initStr;
			while(_curSymbol < _json.Length)
			{
				var s = next();
				switch (_curToken)
				{
					case Token.STRING:
						str += s;
						break;
					case Token.QUOTE:
						return str;
					default:
						_curSymbol--;
						return str;
				}
			}

			return str;
		}
		
		/// <summary>
		/// Read next symbol in the string.
		/// </summary>
		/// <returns></returns>
		private static string next()
		{
			if (_curSymbol >= _json.Length) return null;
			var ch = _json[_curSymbol];
			switch (ch)
			{
				case '{':
					_curToken = Token.LC;
					break;
				case '}':
					_curToken = Token.RC;
					break;
				case '[':
					_curToken = Token.LS;
					break;
				case ']':
					_curToken = Token.RS;
					break;
				case ',':
					_curToken = Token.COMA;
					break;
				case ':':
					_curToken = Token.COLUMN;
					break;
				case '"':
					_curToken = Token.QUOTE;
					break;
				default:
					_curToken = Token.STRING;
					break;
			}
			_curSymbol ++;
			_curChar = ch.ToString();
			return _curChar;
		}

		/// <summary>
		/// Parse a json string.
		/// </summary>
		/// <param name="json"></param>
		/// <param name="reduceNesting"></param>
		/// <returns></returns>
		public static JsonObject Read(string json, bool reduceNesting = true)
		{
			if (json == null) return null;
			_json = Regex.Replace(json, @"\s+[^(?:\w+\s+\w+)]", "");
			_curSymbol = 0;
			var res = get_object();
			if (reduceNesting)
			{
				res.ReduceNesting();
				if (res.Name != null)
					res.GetInBox();
			}
			return res;
		}

		/// <summary>
		/// Parse by the file path.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="reduceNesting"></param>
		/// <returns></returns>
		public static JsonObject ReadPath(string path, bool reduceNesting = true)
		{
			return Read(System.IO.File.ReadAllText(@path), reduceNesting);
		}

		/// <summary>
		/// Add extention method on strings to parse them into JsonPbjects.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="reduceNesting"></param>
		/// <returns></returns>
		public static JsonObject Json(this string str, bool reduceNesting = true)
		{
			return Read(str, reduceNesting);
		}
	}
}