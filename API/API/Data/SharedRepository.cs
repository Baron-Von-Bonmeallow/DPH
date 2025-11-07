namespace API.Data
{
    public class SharedRepository
    {
        private readonly DataContext _context;
        public SharedRepository(DataContext context)
        {
            _context = context;
        }
        public bool Exists<T>(Guid id) where T : class
        {
            var entity = _context.Set<T>().Find(id);
            return entity != null;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);

        }
        public void Remove<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        }
}
