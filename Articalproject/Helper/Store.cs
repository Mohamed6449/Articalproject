namespace Articalproject.Helper
{
	public static class Store
	{
		public static List<Value> Chooses { get; set; }=new List<Value>() { 
		new Value(){Id=true,NameAr="نعم",NameEn="Yes"},
		new Value(){Id=false,NameAr="لا",NameEn="No"},
		
		};


	}
}
