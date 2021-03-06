﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using System;
public class JoyconManager : MonoBehaviour
{

    // Settings accessible via Unity
    public bool EnableIMU = true;
    public bool EnableLocalize = true;

    // Different operating systems either do or don't like the trailing zero
    private const ushort vendor_id = 0x57e;
    private const ushort vendor_id_ = 0x057e;
    private const ushort product_l = 0x2006;
    private const ushort product_r = 0x2007;

    public List<Joycon> joycons = new List<Joycon>(); // Array of all connected Joy-Cons
    private static JoyconManager instance;

    public static List<Joycon> Joycons
    {
        get
        {
            if (!instance) instance = FindObjectOfType<JoyconManager>();
            if (!instance || instance.joycons == null) return new List<Joycon>();

            return instance.joycons;
        }
    }

    void Awake()
    {
        instance = this;
        int i = 0;

        joycons = new List<Joycon>();
        bool isLeft = false;
        HIDapi.hid_init();

        IntPtr ptr = HIDapi.hid_enumerate(vendor_id, 0x0);
        IntPtr top_ptr = ptr;

        if (ptr == IntPtr.Zero)
        {
            ptr = HIDapi.hid_enumerate(vendor_id_, 0x0);
            if (ptr == IntPtr.Zero)
            {
                HIDapi.hid_free_enumeration(ptr);
                Debug.Log("No Joy-Cons found!");
            }
        }
        hid_device_info enumerate;
        while (ptr != IntPtr.Zero)
        {
            enumerate = (hid_device_info)Marshal.PtrToStructure(ptr, typeof(hid_device_info));

            if (enumerate.product_id == product_l || enumerate.product_id == product_r)
            {
                if (enumerate.product_id == product_l)
                {
                    isLeft = true;
                    //Debug.Log("Left Joy-Con connected.");
                }
                else if (enumerate.product_id == product_r)
                {
                    isLeft = false;
                    //Debug.Log("Right Joy-Con connected.");
                }
                else
                {
                    //Debug.Log("Non Joy-Con input device skipped.");
                }
                IntPtr handle = HIDapi.hid_open_path(enumerate.path);
                HIDapi.hid_set_nonblocking(handle, 1);
                joycons.Add(new Joycon(handle, EnableIMU, EnableLocalize & EnableIMU, 0.05f, isLeft, enumerate.serial_number));
                ++i;
            }
            ptr = enumerate.next;
        }
        HIDapi.hid_free_enumeration(top_ptr);
    }

    void OnEnable()
    {
        Awake();
    }

    void Start()
    {
        for (int i = 0; i < joycons.Count; ++i)
        {
            Joycon jc = joycons[i];
            byte LEDs = 0x0;
            LEDs |= (byte)(0x1 << i);
            jc.Attach(leds_: LEDs);
            jc.Begin();
        }
    }

    void Update()
    {
        for (int i = 0; i < joycons.Count; ++i)
        {
            joycons[i].Update();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < joycons.Count; ++i)
        {
            joycons[i].Detach();
        }
    }
}