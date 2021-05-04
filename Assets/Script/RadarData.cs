using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
[Serializable]
public class ClubResult
{
    public double club_path_angle;
    public long recv_time;
    public double attack_angle;
    public double club_speed;
    public double swing_plane_angle;
    public double swing_direction;
}

[Serializable]
public class CarryResult
{
    public double ball_flight2;
    public int ball_flight1;
    public long recv_time;
    public double carry_total;
    public double carry;
}

[Serializable]
public class BallResult
{
    public double launch_angle;
    public long recv_time;
    public int side_spin;
    public double speed;
    public double back_spin;
    public double direction;
}

[Serializable]
public class RadarData
{
    public double PathHeight;
    public ClubResult ClubResult;
    public List<int> Index;
    public CarryResult CarryResult;
    public string Type;
    public List<double> X;
    public List<double> Y;
    public List<double> Z;
    public BallResult BallResult;
    public double APEX;

    public static RadarData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<RadarData>(jsonString);
    }
}
