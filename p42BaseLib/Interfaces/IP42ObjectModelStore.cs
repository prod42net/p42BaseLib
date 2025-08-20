namespace p42BaseLib.Interfaces;

public interface IP42ObjectModelStore
{
    
    // todo: might be good for caching ...
    // Dictionary<string,object> Properties { get; }

    /// Gets the total number of objects in the store, optionally filtered by a specified prefix.
    /// <param name="prefix">An optional string specifying the prefix to filter the objects.
    /// If null or not specified, the count of all objects is returned.</param>
    /// <returns>The total number of objects matching the specified criteria.</returns>
    int NumberOfObject(string? prefix = null);

    /// Retrieves an object of the specified type from the store based on the given name and optional prefix.
    /// <param name="name">The name of the object to retrieve.</param>
    /// <param name="prefix">An optional prefix used to locate the object. Defaults to null.</param>
    /// <typeparam name="T">The type of the object to retrieve.</typeparam>
    /// <returns>A task representing the asynchronous operation. The task result contains the object of type T if found; otherwise, null.</returns>
    Task<T?> Get<T>(string name,string? prefix = null) where T : class;

    /// <summary>
    /// Retrieves all objects of the specified type and with the given name and optional prefix.
    /// </summary>
    /// <typeparam name="T">The type of objects to be retrieved.</typeparam>
    /// <param name="name">The name associated with the objects to retrieve. Can be empty to retrieve all objects.</param>
    /// <param name="prefix">An optional prefix to filter objects.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of objects of type <typeparamref name="T"/> that match the criteria.</returns>
    Task<List<T>> GetAll<T>(string name, string? prefix = null);

    /// <summary>
    /// Adds a new object of type <typeparamref name="T"/> to the store with the specified name and optional prefix.
    /// </summary>
    /// <typeparam name="T">The type of the object being added. Must be a reference type.</typeparam>
    /// <param name="model">The object to be added to the store.</param>
    /// <param name="name">The name associated with the object.</param>
    /// <param name="prefix">An optional prefix to group or categorize the object.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added object if successful, or null otherwise.</returns>
    Task<T?> Add<T>(T model,string name,string? prefix = null) where T : class;

    /// <summary>
    /// Deletes an object identified by the specified name and optional prefix from the store.
    /// </summary>
    /// <param name="name">The name of the object to delete.</param>
    /// <param name="prefix">An optional prefix used to identify the object.</param>
    /// <returns>True if the object was successfully deleted; otherwise, false.</returns>
    bool Delete(string name,string? prefix = null);

    /// Updates an existing object in the store with a new instance of the specified model type.
    /// <typeparam name="T">The type of the object to update.</typeparam>
    /// <param name="name">The unique identifier or name of the object to update.</param>
    /// <param name="model">The new instance of the object to replace the existing one.</param>
    /// <param name="prefix">An optional prefix to organize or group the object (default is null).</param>
    /// <returns>Returns true if the object was successfully updated; false otherwise.</returns>
    bool Update<T>(string name, T model,string? prefix = null) where T : class;
}