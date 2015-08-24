using System;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal static class PropertyKeys
    {
        public static readonly PropertyKey PKEY_DEVICE_INTERFACE_FRIENDLY_NAME = new PropertyKey(new Guid("026E516E-B814-414B-83CD-856D6FEF4822"), 2);

        public static readonly PropertyKey PKEY_AUDIO_ENDPOINT_FORM_FACTOR = new PropertyKey(new Guid("1DA5D803-D492-4EDD-8C23-E0C0FFEE7F0E"), 0);

        public static readonly PropertyKey PKEY_AUDIO_ENDPOINT_CONTROL_PANEL_PAGE_PROVIDER = new PropertyKey(new Guid("1DA5D803-D492-4EDD-8C23-E0C0FFEE7F0E"), 1);

        public static readonly PropertyKey PKEY_AUDIO_ENDPOINT_ASSOCIATION = new PropertyKey(new Guid("1DA5D803-D492-4EDD-8C23-E0C0FFEE7F0E"), 2);

        public static readonly PropertyKey PKEY_AUDIO_ENDPOINT_PHYSICAL_SPEAKERS = new PropertyKey(new Guid("1DA5D803-D492-4EDD-8C23-E0C0FFEE7F0E"), 3);

        public static readonly PropertyKey PKEY_AUDIO_ENDPOINT_GUID = new PropertyKey(new Guid("1DA5D803-D492-4EDD-8C23-E0C0FFEE7F0E"), 4);

        public static readonly PropertyKey PKEY_AUDIO_ENDPOINT_DISABLE_SYS_FX = new PropertyKey(new Guid("1DA5D803-D492-4EDD-8C23-E0C0FFEE7F0E"), 5);

        public static readonly PropertyKey PKEY_AUDIO_ENDPOINT_FULL_RANGE_SPEAKERS = new PropertyKey(new Guid("1DA5D803-D492-4EDD-8C23-E0C0FFEE7F0E"), 6);

        public static readonly PropertyKey PKEY_AUDIO_ENDPOINT_SUPPORTS_EVENT_DRIVEN_MODE = new PropertyKey(new Guid("1DA5D803-D492-4EDD-8C23-E0C0FFEE7F0E"), 7);

        public static readonly PropertyKey PKEY_AUDIO_ENDPOINT_JACK_SUB_TYPE = new PropertyKey(new Guid("1DA5D803-D492-4EDD-8C23-E0C0FFEE7F0E"), 8);

        public static readonly PropertyKey PKEY_AUDIO_ENGINE_DEVICE_FORMAT = new PropertyKey(new Guid("F19F064D-082C-4E27-BC73-6882A1BB8E4C"), 0);

        public static readonly PropertyKey PKEY_AUDIO_ENGINE_OEM_FORMAT = new PropertyKey(new Guid("E4870E26-3CC5-4CD2-BA46-CA0A9A70ED04"), 3);

        public static readonly PropertyKey PKEY_DEVICE_FRIENDLY_NAME = new PropertyKey(new Guid("A45C254E-DF1C-4EFD-8020-67D146A850E0"), 14);

        public static readonly PropertyKey PKEY_DEVICE_DESCRIPTION = new PropertyKey(new Guid("A45C254E-DF1C-4EFD-8020-67D146A850E0"), 2);

        public static readonly PropertyKey PKEY_DEVICE_ICON = new PropertyKey(new Guid("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), 12);

        public static readonly PropertyKey PKEY_SYSTEM_NAME = new PropertyKey(new Guid("B3F8FA53-0004-438E-9003-51A46E139BFC"), 6);
    }
}