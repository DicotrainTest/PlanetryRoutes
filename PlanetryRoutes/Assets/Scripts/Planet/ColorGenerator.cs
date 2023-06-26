using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator {

    ColorSettings settings;

    public ColorGenerator(ColorSettings settings) {

        this.settings = settings;
    }

    public void UpdateElevation(MinMax elevationMinMax) {

        settings.planetMaterial.SetVector("_ElevationMinMax", new Vector4(elevationMinMax.min, elevationMinMax.max));
    }
}
