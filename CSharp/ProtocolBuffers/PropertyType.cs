//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: PropertyType.proto
namespace ThingModel.Proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PropertyType")]
  public partial class PropertyType : global::ProtoBuf.IExtensible
  {
    public PropertyType() {}
    
    private int _string_key;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"string_key", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int string_key
    {
      get { return _string_key; }
      set { _string_key = value; }
    }
    private ThingModel.Proto.PropertyType.Type _type;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public ThingModel.Proto.PropertyType.Type type
    {
      get { return _type; }
      set { _type = value; }
    }
    private string _name = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private string _description = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"description", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string description
    {
      get { return _description; }
      set { _description = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"Type")]
    public enum Type
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOCATION", Value=0)]
      LOCATION = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"STRING", Value=1)]
      STRING = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DOUBLE", Value=2)]
      DOUBLE = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"INT", Value=3)]
      INT = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BOOLEAN", Value=4)]
      BOOLEAN = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DATETIME", Value=5)]
      DATETIME = 5
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}