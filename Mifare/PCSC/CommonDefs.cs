﻿/* Copyright (c) Microsoft Corporation
 * 
 * All rights reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.  You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT.
 * 
 * See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.
*/

namespace Pcsc.Common
{
    /// <summary>
    /// ICC Device class
    /// </summary>
    public enum DeviceClass : byte
    {
        Unknown             = 0x00,
        StorageClass        = 0x01,  // for PCSC class, there will be subcategory to identify the physical icc
        Iso14443P4          = 0x02,
        MifareDesfire       = 0x03,
    }
}
