# Unsafe code in C#
Writing unmanaged (or unsafe) code in C# creates a context where variables are not managed by the runtime, and therefore, are unknown to the garbage collector. We create such a context using the `unsafe` keyword, either on a class, method or locally. Everything within the context is considered unsafe, and is not managed. Therefore, take care, because the garbage collector may allocate memory in the same addresses as the unmanaged code, or remove data from those memory addresses.

To circumvent that problem, use the `fixed` keyword, which ensures an address is kept fixed and is protected from the garbage collector.

In unsafe code, we can make use of pointers to point to memory addresses. The memory address of a variable `T x` is obtained using the `&` operator:
```
T* pointer = &x;
```
This results in a pointer variable that would dereference into type `T`. To dereference, use the `*` operator preceding the pointer:
```
T value = *pointer;
```
That way we can also write to the address behind the pointer:
```
*pointer = value; // assigns value to the address space referenced by the pointer
```

This way, we can also index arrays:
```
T[] array = new T[] { ... };

T* pointer = &array[0]; // points to first element of array

T first = pointer[0];
T second = pointer[1];
```
And write to arrays:
```
pointer[1] = new T();
```
which is equivalent to (using the dereferencing notation):
```
*(pointer + 1) = new T();
```

When a pointer references a struct or class, we can access members of that object as follows:
```
T x = new T();

T* pointer = &x;

value = (*pointer).Property; // retrieve
(*pointer).Property = value; // assign
```
For this there also exists the `->` operator:
```
value = pointer->Property; // retrieve
pointer->Property = value; // assign
```