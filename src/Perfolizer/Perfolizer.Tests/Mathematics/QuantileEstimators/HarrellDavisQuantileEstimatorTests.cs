using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    public class HarrellDavisQuantileEstimatorTests : QuantileEstimatorTests
    {
        public HarrellDavisQuantileEstimatorTests(ITestOutputHelper output) : base(output)
        {
        }

        private static readonly IDictionary<string, TestData> TestDataMap = new Dictionary<string, TestData>
        {
            {
                "Case1", new TestData(
                    new double[] {0, 25, 50, 75, 100},
                    new Probability[] {0, 0.1, 0.2, 0.25, 0.3, 0.4, 0.5, 0.6, 0.7, 0.75, 0.8, 0.9, 1.0},
                    new[]
                    {
                        0, 4.81290947065674, 13.7443607731199, 19.2481103578583, 25.1415863187833, 37.4702805366232, 50, 62.5297194633768,
                        74.8584136812167, 80.7518896421417, 86.2556392268801, 95.1870905293433, 100
                    }
                )
            },
            {
                // R-script that generates data
                // library(Hmisc)
                // set.seed(42)
                // x <- rnorm(100, 50, 5)
                // probs <- seq(0, 1, by = 0.01)
                // q <- hdquantile(x, probs)
                "Case2", new TestData(
                    new[]
                    {
                        56.8547922357333, 47.1765091430196, 51.8156420566867, 53.1643130248052,
                        52.021341615705, 49.4693774195426, 57.5576099871947, 49.5267048079345,
                        60.0921185693852, 49.6864295047379, 56.5243482711174, 61.4332269635055,
                        43.0556964944383, 48.6060561659131, 49.3333933180317, 53.1797519903504,
                        48.5787353929196, 36.7177228954761, 37.7976653571224, 56.600566728651,
                        48.4668070296076, 41.0934578301, 49.1404132212019, 56.073373495863,
                        59.4759673063248, 47.847654341969, 48.7136530861554, 41.1841845740261,
                        52.3004867741564, 46.8000256201994, 52.2772506162061, 53.5241866861441,
                        55.1755176098496, 46.9553681229639, 52.5247756164899, 41.4149566046333,
                        46.0777049581025, 45.7454620291174, 37.9289617502668, 50.1806130344613,
                        51.0299930010013, 48.1947135072567, 53.7908161784976, 46.3664758646171,
                        43.1585947779035, 52.1640901294436, 45.9430341190666, 57.2205063086063,
                        47.8427689869333, 53.278239417011, 51.6096263260197, 46.0808052955981,
                        57.8786375989599, 53.2144965285866, 50.448803232998, 51.3827537364573,
                        53.3964440802764, 50.4491644328954, 35.0345495842353, 51.4244147676533,
                        48.1638267862951, 50.926152824328, 52.9091186368275, 56.9986841364634,
                        46.3635397026277, 56.5127131602207, 51.6792405987604, 55.1925304934881,
                        54.6036428414532, 53.6043908143343, 44.7844053071607, 49.5490680669465,
                        53.1175908099977, 45.2323832111383, 47.2858559271307, 52.9049824884084,
                        53.840893689173, 52.3188379427008, 45.5711185129516, 44.5010955067607,
                        57.5635350490246, 51.2896071876602, 50.4422011457979, 49.3955173123046,
                        44.0283555241974, 53.0599844902019, 48.9143007712674, 49.0862164683404,
                        54.6667316428558, 54.1088655525412, 56.9605818796714, 47.6191303847266,
                        53.2517428036315, 56.95555228195, 44.4460556027605, 45.6960370656108,
                        44.3413065957311, 42.703930002488, 50.3999127662058, 53.266021698246
                    },
                    new Probability[]
                    {
                        0, 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1,
                        0.11, 0.12, 0.13, 0.14, 0.15, 0.16, 0.17, 0.18, 0.19, 0.2, 0.21,
                        0.22, 0.23, 0.24, 0.25, 0.26, 0.27, 0.28, 0.29, 0.3, 0.31, 0.32,
                        0.33, 0.34, 0.35, 0.36, 0.37, 0.38, 0.39, 0.4, 0.41, 0.42, 0.43,
                        0.44, 0.45, 0.46, 0.47, 0.48, 0.49, 0.5, 0.51, 0.52, 0.53, 0.54,
                        0.55, 0.56, 0.57, 0.58, 0.59, 0.6, 0.61, 0.62, 0.63, 0.64, 0.65,
                        0.66, 0.67, 0.68, 0.69, 0.7, 0.71, 0.72, 0.73, 0.74, 0.75, 0.76,
                        0.77, 0.78, 0.79, 0.8, 0.81, 0.82, 0.83, 0.84, 0.85, 0.86, 0.87,
                        0.88, 0.89, 0.9, 0.91, 0.92, 0.93, 0.94, 0.95, 0.96, 0.97, 0.98,
                        0.99, 1
                    },
                    new[]
                    {
                        35.0345495842353, 35.8665470068393, 37.0492818595434, 38.2011795736415,
                        39.2746552410464, 40.256109743906, 41.1134086323857, 41.836544474867,
                        42.4426412161145, 42.9571732297402, 43.4009039307175, 43.7880492225599,
                        44.1290086615892, 44.4324717809119, 44.7058913908746, 44.9552400136658,
                        45.1849474601153, 45.3983021471594, 45.598114660669, 45.7872807838542,
                        45.9689878385665, 46.1465304445227, 46.3228875722661, 46.5002811937488,
                        46.6798931785275, 46.8618172351466, 47.0452259992295, 47.2286743951137,
                        47.4104452354453, 47.5888596990133, 47.7625064694369, 47.9303746482844,
                        48.0918990869188, 48.2469402324582, 48.3957251218213, 48.538774137293,
                        48.67683178313, 48.810810932084, 48.9417506426675, 49.070780010505,
                        49.1990768488069, 49.3278116735592, 49.4580740796712, 49.5907876679314,
                        49.7266275001795, 49.865957241708, 50.0088002452855, 50.1548510237768,
                        50.303524054992, 50.454029286405, 50.6054603836255, 50.7568828230997,
                        50.9074126213311, 51.0562804241377, 51.2028781300857, 51.3467858441725,
                        51.4877766016192, 51.6257962880472, 51.7609175717075, 51.8932698940412,
                        52.0229524453141, 52.1499428444522, 52.274019647676, 52.3947199640351,
                        52.5113522397392, 52.6230771328159, 52.7290564282859, 52.8286536071956,
                        52.9216545362854, 53.0084679634636, 53.0902667378516, 53.1690422438322,
                        53.2475629377154, 53.329246931033, 53.4179718177541, 53.5178481652509,
                        53.6329757966491, 53.7671874039103, 53.9237679032075, 54.1051274520671,
                        54.3124094550212, 54.5450408536013, 54.800285497896, 55.0729345214826,
                        55.3553302014041, 55.6379222490652, 55.9104546245791, 56.1636794359808,
                        56.3912623633759, 56.5913968746182, 56.7677005553656, 56.9293335094683,
                        57.0910061666847, 57.2742817437268, 57.5109207356806, 57.8450445439088,
                        58.3259517813988, 58.9889424161316, 59.834581951651, 60.7696586964613,
                        61.4332269635055
                    }
                )
            },
            {
                // R-script that generates data
                // library(Hmisc)
                // set.seed(42)
                // x <- c(10 + rbeta(70, 10, 1) * 100, 50 + rbeta(70, 2, 15) * 100)
                // probs <- seq(0, 1, by = 0.01)
                // q <- hdquantile(x, probs)
                "Case3", new TestData(
                    new[]
                    {
                        104.71266481958, 106.547365239291, 105.038172248937, 99.4083487859505,
                        109.305913682037, 99.580601312095, 109.777912203776, 100.047288481264,
                        108.949587943969, 96.4845002986208, 98.9894392007521, 106.564650487893,
                        96.3811443619497, 108.98029933179, 95.950269477041, 98.3692893862343,
                        108.751416410323, 109.70187611127, 93.3391994652827, 96.8847310900613,
                        105.451715063034, 87.9415525502157, 106.931357198257, 108.261747714475,
                        105.761820454489, 83.4139814444016, 101.37440834687, 103.288951944087,
                        94.8514983946836, 107.191617193855, 85.3072842295771, 105.25358720541,
                        109.203120530136, 93.3169893528397, 106.673168594764, 104.367948536203,
                        83.4349073076172, 106.603277249871, 102.025926782337, 103.965491169005,
                        106.799498444971, 102.026773616316, 104.041248690101, 94.7220447194544,
                        103.07734939631, 106.250556760691, 108.94483163048, 104.486585182634,
                        108.272344660067, 107.871990508552, 107.011735870926, 73.5468193758375,
                        99.6471088963838, 106.49459686133, 90.6079906449014, 106.245569760308,
                        107.238324169671, 105.477811368152, 79.8184741235496, 109.307614670131,
                        103.793014144946, 102.010434187049, 99.1826359430983, 106.428577835557,
                        97.6027320006302, 98.1895530247875, 107.918330481413, 107.477131287265,
                        92.2718849977342, 106.644268359578, 54.2271384396102, 55.4498883821709,
                        54.1345494602263, 52.867818217569, 62.5703074011247, 69.9838574595559,
                        77.261161542512, 54.7260827082969, 56.5617013785135, 62.837475359364,
                        58.1952447900618, 60.6186828905358, 63.7377450328999, 62.9823392890992,
                        61.5214112651435, 62.8645281595024, 57.746249739752, 57.6165247444775,
                        52.2426145454959, 58.489286867841, 57.5588254834201, 53.2093416879252,
                        63.1766643339177, 52.7533090071472, 52.8301067265479, 63.0415753551562,
                        54.7578537082383, 61.7468855535579, 58.1517316454, 56.8932617637745,
                        51.3771498074672, 53.740865646504, 63.6406775601129, 68.6627491611739,
                        57.6946452555337, 61.7653212670617, 78.986974531575, 54.288368406423,
                        60.6638563763055, 80.8240457252996, 57.8560326653454, 61.2609233789704,
                        60.9477769643431, 58.0335963573481, 61.4796566655369, 58.2563433494578,
                        57.7771810142516, 58.1162748319045, 70.6919175463623, 54.4745598132247,
                        61.3722964445609, 52.5680654627768, 59.332520777879, 57.8692841987557,
                        61.6889761393117, 55.7804762377759, 57.4176639625882, 60.1509084535821,
                        54.4034582832672, 63.4598892062613, 58.6366287467455, 69.0000950589345,
                        86.8272985495699, 54.7770135323528, 67.0461170406262, 62.6213591272305,
                        74.2310234302153, 59.6914364143579, 73.1245718875274, 52.0144312008348
                    },
                    new Probability[]
                    {
                        0, 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1,
                        0.11, 0.12, 0.13, 0.14, 0.15, 0.16, 0.17, 0.18, 0.19, 0.2, 0.21,
                        0.22, 0.23, 0.24, 0.25, 0.26, 0.27, 0.28, 0.29, 0.3, 0.31, 0.32,
                        0.33, 0.34, 0.35, 0.36, 0.37, 0.38, 0.39, 0.4, 0.41, 0.42, 0.43,
                        0.44, 0.45, 0.46, 0.47, 0.48, 0.49, 0.5, 0.51, 0.52, 0.53, 0.54,
                        0.55, 0.56, 0.57, 0.58, 0.59, 0.6, 0.61, 0.62, 0.63, 0.64, 0.65,
                        0.66, 0.67, 0.68, 0.69, 0.7, 0.71, 0.72, 0.73, 0.74, 0.75, 0.76,
                        0.77, 0.78, 0.79, 0.8, 0.81, 0.82, 0.83, 0.84, 0.85, 0.86, 0.87,
                        0.88, 0.89, 0.9, 0.91, 0.92, 0.93, 0.94, 0.95, 0.96, 0.97, 0.98,
                        0.99, 1
                    },
                    new[]
                    {
                        51.3771498074672, 51.8137855519871, 52.2787282734171, 52.6202979267238,
                        52.9213708393491, 53.2337019270662, 53.5613985119458, 53.8820771309612,
                        54.1802644202967, 54.4643057827845, 54.7587412443154, 55.0865830162641,
                        55.4559146154749, 55.8562732267903, 56.2637389599039, 56.6506780850274,
                        56.9953567038693, 57.2877988321391, 57.5307048780513, 57.7365676823582,
                        57.9231521642653, 58.1091542586953, 58.3108865530214, 58.540070361462,
                        58.8025776450753, 59.0980892646265, 59.4207610834606, 59.7608943223941,
                        60.1073262929234, 60.4499957721352, 60.7820857703884, 61.1013518234188,
                        61.4105914727192, 61.7175091273643, 62.0343290825138, 62.3773778858251,
                        62.7665956900039, 63.2247169460325, 63.7758328074726, 64.44325195667,
                        65.2469177604217, 66.2009340915424, 67.3118306157282, 68.5780038645938,
                        69.9903906149897, 71.5340437275848, 73.1900544338097, 74.9372688168456,
                        76.7534358358598, 78.6156916707731, 80.5005282371329, 82.3835554393,
                        84.2394292181477, 86.042276016392, 87.7667976379807, 89.3900081080201,
                        90.8932980310152, 92.2643398253269, 93.4983279701095, 94.5982159660246,
                        95.5738997590979, 96.4405789800761, 97.2166848155719, 97.9217538207736,
                        98.5745010848554, 99.1912078035076, 99.7844736961842, 100.362407880554,
                        100.928382407916, 101.481464924582, 102.017534347366, 102.530894156747,
                        103.016018911934, 103.468994144565, 103.888284794154, 104.274671843911,
                        104.630458012584, 104.958273247449, 105.259938874046, 105.535837130241,
                        105.785074200586, 106.006447795781, 106.199908890321, 106.367964860412,
                        106.516441250362, 106.654256309447, 106.792263129271, 106.941546937083,
                        107.111640931277, 107.30897343066, 107.535674692174, 107.788737515096,
                        108.059450878136, 108.333459997806, 108.593126420353, 108.824010167406,
                        109.023555857231, 109.207818205976, 109.410441035785, 109.636831221601,
                        109.777912203776
                    }
                )
            },
            {
                "WeightedCase1", new TestData(
                    new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                    new Probability[] {0.5},
                    new[] {3.0},
                    new[] {0.0, 1.0, 1.0, 1.0, 0.0})
            },
            {
                "WeightedCase2", new TestData(
                    new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                    new Probability[] {0.5},
                    new[] {2.0},
                    new[] {1.0, 1.0, 1.0, 0.0, 0.0})
            },
            {
                "WeightedCase3", new TestData(
                    new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                    new Probability[] {0.5},
                    new[] {2.419753079637479},
                    new[] {1.0, 1.0, 0.0, 0.0, 1.0})
            },
            {
                "WeightedCase4", new TestData(
                    new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                    new Probability[] {0.5},
                    new[] {2.1410844421648347},
                    new[] {1.0, 1.0, 0.2, 0.4, 0.4})
            },
            {
                "WeightedCase5", new TestData(
                    new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                    new Probability[] {0.5},
                    new[] {3.0},
                    new[] {1.0, 0.0, 0.0, 0.0, 1.0})
            },
            {
                "WeightedCase6", new TestData(
                    new[] {1.0, 2.0, 3.0, 4.0, 5.0},
                    new Probability[] {0.5},
                    new[] {2.990671779712344},
                    new[] {1.0, 0.01, 0.0, 0.0, 1.0})
            }
        };

        [UsedImplicitly] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void HarrellDavisQuantileEstimatorTest([NotNull] string testDataKey)
        {
            Check(HarrellDavisQuantileEstimator.Instance, TestDataMap[testDataKey]);
        }
        
        [Theory]
        [InlineData(0.99, 0.9, 1.0)]
        [InlineData(0.80, 0.7, 1.0)]
        [InlineData(0.50, 0.3, 0.8)]
        public void MaritzJarrettConfidenceIntervalTest(double confidenceLevel, double minSuccessRate, double maxSuccessRate)
        {
            var random = new Random(42);
            var distribution = new BetaDistribution(2, 10);
            var generator = distribution.Random(random);
            var estimator = HarrellDavisQuantileEstimator.Instance;
            double median = distribution.Median;
            const int iterations = 100;
            for (int n = 5; n <= 10; n++)
            {
                int successCount = 0;
                for (int i = 0; i < iterations; i++)
                {
                    var sample = generator.Next(n).ToSample();
                    var confidenceInterval = estimator
                        .GetQuantileConfidenceIntervalEstimator(sample, 0.5)
                        .GetConfidenceInterval(confidenceLevel);
                    if (confidenceInterval.Contains(median))
                        successCount++;
                }

                double successRate = successCount * 1.0 / iterations;
                Output.WriteLine($"n = {n}, successRate = {successRate:N2}");
                Assert.True(minSuccessRate <= successRate && successRate <= maxSuccessRate);
            }
        }
    }
}