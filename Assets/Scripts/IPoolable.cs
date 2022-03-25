namespace Pooling
{
	public interface IPoolable
	{
		/// <summary>
		/// Use this for initializing/resetting a previously pooled object
		/// </summary>
		public void Initialize();
	}
}