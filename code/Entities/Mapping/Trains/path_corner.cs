﻿[Library("path_corner")]
[HammerEntity]
[Title("path_corner"), Category("Legacy"), Icon("conversion_path")]
public partial class path_corner : Entity
{
    [Property("target"), FGDType("target_destination")]
    public string Target { get; set; } = "";

    // stub
    [Input]
    void Activate()
    {

    }
}