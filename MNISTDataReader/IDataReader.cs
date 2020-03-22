namespace MNISTDataReader
{
	public interface IDataReader<T>
	{
		T Read(int index);
	}
}
