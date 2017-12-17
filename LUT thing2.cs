// These are done as s11.52 fixed points for precision (sign bit, 11 int bits, 53 frac bits)
        private const long SinCosScale = 0x0009B74EDA8435DF;
        private const long PI_CORDIC = 0x003363F6A8885A30;
        private const long PI_2_CORDIC = PI_CORDIC * 2;

        public static readonly long [] CordicTable = {
            0x000c90fdaa22168c, 0x00076b19c1586ed3, 0x0003eb6ebf25901b,
            0x0001fd5ba9aac2f6, 0x0000ffaaddb967ef, 0x00007ff556eea5d8,
            0x00003ffeaab776e5, 0x00001fffd555bbba, 0x00000ffffaaaaddd,
            0x000007ffff55556e, 0x000003ffffeaaaab, 0x000001fffffd5555,
            0x000000ffffffaaaa, 0x0000007ffffff555, 0x0000003ffffffeaa,
            0x0000001fffffffd5, 0x0000000ffffffffa, 0x00000007ffffffff,
            0x00000003ffffffff, 0x00000001ffffffff, 0x00000000ffffffff,
            0x000000007fffffff, 0x000000003fffffff, 0x000000001fffffff,
            0x000000000fffffff, 0x0000000007ffffff, 0x0000000003ffffff,
            0x0000000002000000, 0x0000000001000000, 0x0000000000800000,
            0x0000000000400000, 0x0000000000200000, 0x0000000000100000,
            0x0000000000080000, 0x0000000000040000, 0x0000000000020000,
            0x0000000000010000, 0x0000000000008000, 0x0000000000004000,
            0x0000000000002000, 0x0000000000001000, 0x0000000000000800,
            0x0000000000000400, 0x0000000000000200, 0x0000000000000100,
            0x0000000000000080, 0x0000000000000040, 0x0000000000000020,
            0x0000000000000010, 0x0000000000000008, 0x0000000000000004,
            0x0000000000000002, 0x0000000000000001,
        };

        public static Accum Cos (Accum input) {
            /* Force input into range. */
            if (input < -PI_2) return -Cos (input + PI);
            if (input > PI_2) return -Cos (input - PI);

            long x = 1 << 52, y = 0, x0, z = input.Value << 36;

            for (int j = 0; j != CordicTable.Length; ++j) {
                x0 = x;

                if (z < 0) {
                    x += y >> j;
                    y -= x0 >> j;
                    z += CordicTable [j];
                } else {
                    x -= y >> j;
                    y += x0 >> j;
                    z -= CordicTable [j];
                }
            }

            return Accum.MakeAccum ((int) ((x * SinCosScale) >> 36));
        }

        public static Accum Sin (Accum input) {
            /* Force input into range. */
            if (input < -PI_2) return -Sin (input + PI);
            if (input > PI_2) return -Sin (input - PI);

            long x = 1 << 52, y = 0, x0, z = input.Value << 36;

            for (int j = 0; j != CordicTable.Length; ++j) {
                x0 = x;

                if (z < 0) {
                    x += y >> j;
                    y -= x0 >> j;
                    z += CordicTable [j];
                } else {
                    x -= y >> j;
                    y += x0 >> j;
                    z -= CordicTable [j];
                }F
            }

            return Accum.MakeAccum ((int) ((y * SinCosScale) >> 36));
        }

        public static Accum Tan (Accum input) {
            long x = 1 << 52, y = 0, x0, z = input.Value;

            /* Force input into range. */
            while (z < -PI_2_CORDIC) z += PI_CORDIC;
            while (z > PI_2_CORDIC) z -= PI_CORDIC;

            for (int j = 0; j != CordicTable.Length; ++j) {
                x0 = x;

                if (z < 0) {
                    x += y >> j;
                    y -= x0 >> j;
                    z += CordicTable [j];
                    Console.WriteLine ("A");
                } else {
                    x -= y >> j;
                    y += x0 >> j;
                    z -= CordicTable [j];
                    Console.WriteLine ("B");
                }
            }

            return Accum.MakeAccum ((int) ((y / x) >> 36));
        }

        public static Accum Atan (Accum input) {
            long x = 1 << 52, z = 0, x0, y = input.Value;

            for (int j = 0; j != CordicTable.Length; ++j) {
                x0 = x;

                if (y < 0) {
                    x -= y >> j;
                    y += x0 >> j;
                    z -= CordicTable [j];
                } else {
                    x += y >> j;
                    y -= x0 >> j;
                    z += CordicTable [j];
                }
            }

            return Accum.MakeAccum ((int) (z >> 36));
        }

        public static Accum Atan2 (Accum inputY, Accum inputX) {
            if (inputX < 0) {
                if (inputY < 0)
                    return Atan2 (-inputY, -inputX) - PI;
                else
                    return Atan2 (-inputY, -inputX) + PI;
            } else if (inputX == 0) {
                if (inputY < 0) return -PI_2;
                if (inputY > 0) return PI_2;

                return inputY; /* Return 0 with the same sign as inputY. Since inputY is zero... */
            }

            long x = inputX.Value << 36, y = inputY.Value << 36, z = 0, x0;

            for (int j = 0; j != CordicTable.Length; ++j) {
                x0 = x;

                if (y < 0) {
                    x -= y >> j;
                    y += x0 >> j;
                    z -= CordicTable [j];
                } else {
                    x += y >> j;
                    y -= x0 >> j;
                    z += CordicTable [j];
                }
            }

            return Accum.MakeAccum ((int) (z >> 36));
        }