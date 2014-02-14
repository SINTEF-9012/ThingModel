//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Property.proto
namespace ThingModel.Proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Property")]
  internal partial class Property : global::ProtoBuf.IExtensible
  {
    public Property() {}
    
    private int _string_key;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"string_key", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int string_key
    {
      get { return _string_key; }
      set { _string_key = value; }
    }
    private ThingModel.Proto.Property.Type _type;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public ThingModel.Proto.Property.Type type
    {
      get { return _type; }
      set { _type = value; }
    }
    private ThingModel.Proto.Property.Location _location_value = null;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"location_value", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ThingModel.Proto.Property.Location location_value
    {
      get { return _location_value; }
      set { _location_value = value; }
    }
    private ThingModel.Proto.Property.String _string_value = null;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"string_value", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ThingModel.Proto.Property.String string_value
    {
      get { return _string_value; }
      set { _string_value = value; }
    }
    private double _double_value = default(double);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"double_value", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(double))]
    public double double_value
    {
      get { return _double_value; }
      set { _double_value = value; }
    }
    private int _int_value = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"int_value", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int int_value
    {
      get { return _int_value; }
      set { _int_value = value; }
    }
    private bool _boolean_value = default(bool);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"boolean_value", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool boolean_value
    {
      get { return _boolean_value; }
      set { _boolean_value = value; }
    }
    private long _datetime_value = default(long);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"datetime_value", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long datetime_value
    {
      get { return _datetime_value; }
      set { _datetime_value = value; }
    }
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Location")]
  internal partial class Location : global::ProtoBuf.IExtensible
  {
    public Location() {}
    
    private double _x;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public double x
    {
      get { return _x; }
      set { _x = value; }
    }
    private double _y;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public double y
    {
      get { return _y; }
      set { _y = value; }
    }
    private double _z = (double)0;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"z", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((double)0)]
    public double z
    {
      get { return _z; }
      set { _z = value; }
    }
    private int _string_system = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"string_system", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int string_system
    {
      get { return _string_system; }
      set { _string_system = value; }
    }
    private bool _z_null = (bool)false;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"z_null", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue((bool)false)]
    public bool z_null
    {
      get { return _z_null; }
      set { _z_null = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"String")]
  internal partial class String : global::ProtoBuf.IExtensible
  {
    public String() {}
    
    private string _value = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"value", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string value
    {
      get { return _value; }
      set { _value = value; }
    }
    private int _string_value = (int)0;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"string_value", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int string_value
    {
      get { return _string_value; }
      set { _string_value = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"Type")]
    public enum Type
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOCATION_POINT", Value=0)]
      LOCATION_POINT = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOCATION_LATLNG", Value=1)]
      LOCATION_LATLNG = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOCATION_EQUATORIAL", Value=2)]
      LOCATION_EQUATORIAL = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"STRING", Value=3)]
      STRING = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DOUBLE", Value=4)]
      DOUBLE = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"INT", Value=5)]
      INT = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BOOLEAN", Value=6)]
      BOOLEAN = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DATETIME", Value=7)]
      DATETIME = 7
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}