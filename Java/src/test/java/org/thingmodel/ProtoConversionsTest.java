package org.thingmodel;

import org.junit.Before;
import org.thingmodel.proto.FromProtobuf;
import org.thingmodel.proto.ToProtobuf;

public class ProtoConversionsTest {
    private Wharehouse _wharehouse;
    private FromProtobuf _fromProtobuf;
    private ToProtobuf _toProtobuf;

    @Before
    public void setUp() throws Exception {
        _wharehouse = new Wharehouse();
        _fromProtobuf = new FromProtobuf(_wharehouse);
        _toProtobuf = new ToProtobuf();
    }
}
