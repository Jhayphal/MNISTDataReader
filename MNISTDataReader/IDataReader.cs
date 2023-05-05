using System.Collections.Generic;

namespace MNISTDataReader
{
	public interface IDataReader<out T> : IEnumerable<T>
	{
		T Read(int index);

		IEnumerable<T> GetScope(int begin, int end);
	}
}
