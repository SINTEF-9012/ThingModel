//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Thing.proto
// Note: requires additional types generated from: Property.proto
namespace ThingModel.Proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Thing")]
  public partial class Thing : global::ProtoBuf.IExtensible
  {
    public Thing() {}
    
    private int _string_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"string_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int string_id
    {
      get { return _string_id; }
      set { _string_id = value; }
    }
    private int _string_type_name;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"string_type_name", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int string_type_name
    {
      get { return _string_type_name; }
      set { _string_type_name = value; }
    }
    private readonly global::System.Collections.Generic.List<ThingModel.Proto.Property> _properties = new global::System.Collections.Generic.List<ThingModel.Proto.Property>();
    [global::ProtoBuf.ProtoMember(3, Name=@"properties", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ThingModel.Proto.Property> properties
    {
      get { return _properties; }
    }
  
    private readonly global::System.Collections.Generic.List<int> _connections = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(4, Name=@"connections", DataFormat = global::ProtoBuf.DataFormat.TwosComplement, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
    public global::System.Collections.Generic.List<int> connections
    {
      get { return _connections; }
    }
  
    private bool _connections_change = (bool)false;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"connections_change", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue((bool)false)]
    public bool connections_change
    {
      get { return _connections_change; }
      set { _connections_change = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}