using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NUnit.Framework;

namespace NetTopologySuite.Samples.Tests.Github
{
    [TestFixture]
    public class Issue181Fixture
    {
        /// <summary>
        /// Same results with JTS, see: https://github.com/NetTopologySuite/NetTopologySuite/issues/181
        /// </summary>
        [Test]
        public void expected_exception_is_thrown_using_difference()
        {
            WKTReader reader = new WKTReader();
            IGeometry g1 = reader.Read(
                @"MULTIPOLYGON (((-6254384.9272019733 -2428784.4316727975, -6254448.2889524475 -2428784.4324241709, -6254399.1165902149 -2428657.1297944547, -6254384.9272019733 -2428784.4316727975)), ((-6254289.7647291981 -2428374.0280918181, -6254076.9438697845 -2428906.9316744655, -6254243.0436896412 -2428910.1951122279, -6254417.483366685 -2428913.62240691, -6254463.6874999283 -2428837.6061908188, -6254325.4337456506 -2428846.8321458441, -6254362.340771623 -2428561.9206525073, -6254289.7647291981 -2428374.0280918181)), ((-6254073.3327726927 -2428940.0660867151, -6254140.5877170712 -2429108.987212894, -6254344.0507978722 -2429034.4355382724, -6254399.2597350832 -2428943.6043894938, -6254073.3327726927 -2428940.0660867151)), ((-6254753.3077126574 -2428361.1156448247, -6254698.72562374 -2428213.1250561425, -6254332.4671062678 -2428330.1534979446, -6254488.1171720289 -2428797.4138724417, -6254753.3077126574 -2428361.1156448247)), ((-6254004.2787381727 -2428950.0691508525, -6254318.7635007482 -2428263.0369477733, -6254122.8733536266 -2428231.3153878264, -6253776.8295307625 -2428881.4327149931, -6251457.7266220115 -2428679.5463844533, -6251326.4963204153 -2429188.8054935131, -6253531.782834392 -2429376.9298687372, -6254004.2787381727 -2428950.0691508525)))");
            Assert.AreEqual(true, g1.IsValid);
            IGeometry g2 = reader.Read(
                @"MULTIPOLYGON (((-6254444.050800845 -2428784.4323739046, -6254448.2889524437 -2428784.4324241676, -6254444.0508008441 -2428773.4602475408, -6254438.6335666114 -2428759.4355366551, -6254428.97697854 -2428734.4355366575, -6254419.3203904629 -2428709.4355366589, -6254419.0508008488 -2428708.7375944532, -6254409.6638023909 -2428684.4355366575, -6254400.0072143227 -2428659.435536663, -6254399.1165902093 -2428657.1297944514, -6254398.85958637 -2428659.4355366565, -6254396.07302336 -2428684.4355366593, -6254394.0508008441 -2428702.5781598496, -6254393.2864603419 -2428709.4355366584, -6254390.499897331 -2428734.4355366556, -6254387.7133343164 -2428759.4355366491, -6254384.9272019733 -2428784.4316727975, -6254394.0508008422 -2428784.4317809842, -6254419.0508008394 -2428784.4320774451, -6254444.050800845 -2428784.4323739046)), ((-6254444.0507973628 -2428859.4355378593, -6254444.0507973656 -2428869.9129937063, -6254450.4191986173 -2428859.43553786, -6254463.6874999283 -2428837.6061908188, -6254444.0507973628 -2428838.9165880294, -6254419.0507973675 -2428840.58488903, -6254394.0507973628 -2428842.253190022, -6254369.0507973591 -2428843.9214910129, -6254344.0507973544 -2428845.5897920118, -6254325.4337456506 -2428846.8321458441, -6254327.0395844625 -2428834.4355378533, -6254330.2780485861 -2428809.4355378528, -6254333.5165127087 -2428784.4355378593, -6254336.7549768286 -2428759.4355378575, -6254339.993440954 -2428734.4355378621, -6254343.231905072 -2428709.4355378593, -6254344.0507973712 -2428703.1139278188, -6254346.4703691984 -2428684.4355378631, -6254349.70883332 -2428659.4355378686, -6254352.9472974436 -2428634.4355378672, -6254356.1857615709 -2428609.4355378728, -6254359.42422569 -2428584.4355378705, -6254362.340771623 -2428561.9206525073, -6254361.3808624921 -2428559.4355378714, -6254351.7242744239 -2428534.4355378784, -6254344.0507973786 -2428514.5696261721, -6254342.0676863473 -2428509.4355378719, -6254332.4110982772 -2428484.435537877, -6254322.7545102052 -2428459.435537877, -6254319.0507973842 -2428449.846973083, -6254319.0507973833 -2428459.4355378728, -6254319.0507973805 -2428449.8469730811, -6254313.09792213 -2428434.435537878, -6254303.4413340567 -2428409.4355378794, -6254294.050797388 -2428385.124319992, -6254293.7847459819 -2428384.4355378775, -6254289.7647291981 -2428374.0280918181, -6254285.6084020734 -2428384.4355378742, -6254275.6243793406 -2428409.4355378747, -6254269.0507973833 -2428425.8957917569, -6254265.6403566087 -2428434.4355378747, -6254255.6563338693 -2428459.4355378691, -6254245.6723111346 -2428484.4355378696, -6254244.0507973777 -2428488.4958094717, -6254235.6882884027 -2428509.4355378668, -6254225.7042656615 -2428534.4355378617, -6254219.0507973675 -2428551.0958271823, -6254215.7202429362 -2428559.4355378626, -6254205.7362201922 -2428584.4355378547, -6254195.752197451 -2428609.4355378523, -6254194.0507973619 -2428613.6958449022, -6254185.7681747228 -2428634.4355378537, -6254175.7841519862 -2428659.4355378547, -6254169.0507973526 -2428676.2958626165, -6254165.8001292506 -2428684.4355378463, -6254155.8161065178 -2428709.4355378477, -6254145.8320837785 -2428734.4355378472, -6254144.050797347 -2428738.8958803294, -6254135.84806104 -2428759.4355378412, -6254125.8640383054 -2428784.4355378421, -6254119.0507973451 -2428801.49589804, -6254115.8800155725 -2428809.4355378379, -6254105.8959928416 -2428834.4355378356, -6254095.9119701013 -2428859.4355378351, -6254094.0507973349 -2428864.0959157594, -6254085.9279473675 -2428884.4355378319, -6254076.9438697845 -2428906.9316744655, -6254094.0507973265 -2428907.2677819543, -6254119.05079733 -2428907.758968187, -6254144.0507973321 -2428908.2501544147, -6254169.0507973339 -2428908.7413406391, -6254194.05079734 -2428909.2325268677, -6254204.3834856767 -2428909.4355378374, -6254219.0507973414 -2428909.7237130953, -6254243.0436896412 -2428910.1951122279, -6254244.050797346 -2428910.2148993253, -6254269.0507973451 -2428910.7060855534, -6254294.0507973488 -2428911.1972717838, -6254319.0507973544 -2428911.6884580115, -6254344.050797347 -2428912.1796442387, -6254369.0507973535 -2428912.6708304686, -6254394.05079736 -2428913.1620166982, -6254417.4833666813 -2428913.6224069092, -6254419.05079736 -2428911.0436300607, -6254420.0282270536 -2428909.4355378519, -6254435.2237128373 -2428884.4355378575, -6254444.0507973637 -2428869.9129937077, -6254444.0507973628 -2428859.4355378593)), ((-6254753.3077126583 -2428361.11564482, -6254752.6880535092 -2428359.4355383315, -6254744.050797944 -2428336.0170037593, -6254743.4675197275 -2428334.4355383296, -6254734.24698594 -2428309.4355383338, -6254725.0264521576 -2428284.4355383362, -6254719.050797943 -2428268.2335093888, -6254715.8059183694 -2428259.4355383315, -6254706.5853845831 -2428234.4355383338, -6254698.72562374 -2428213.1250561425, -6254694.0507979412 -2428214.6187758865, -6254669.0507979458 -2428222.6068796609, -6254644.050797943 -2428230.5949834352, -6254632.0311903935 -2428234.4355383315, -6254619.0507979337 -2428238.583087205, -6254594.0507979393 -2428246.5711909803, -6254569.0507979374 -2428254.5592947542, -6254553.7898433041 -2428259.4355383259, -6254544.0507979337 -2428262.5473985276, -6254519.0507979309 -2428270.5355022973, -6254494.0507979263 -2428278.5236060717, -6254475.5484962119 -2428284.4355383213, -6254469.0507979272 -2428286.5117098419, -6254444.0507979225 -2428294.4998136181, -6254419.0507979235 -2428302.4879173888, -6254397.30714911 -2428309.435538311, -6254394.0507979216 -2428310.4760211641, -6254369.0507979179 -2428318.4641249366, -6254344.0507979142 -2428326.452228711, -6254332.4671062678 -2428330.1534979446, -6254333.8935055509 -2428334.4355383092, -6254342.2213070495 -2428359.4355383092, -6254344.0507979114 -2428364.9276566892, -6254350.5491085406 -2428384.4355383068, -6254358.8769100271 -2428409.4355383008, -6254367.2047115248 -2428434.4355383045, -6254369.0507979132 -2428439.9774763263, -6254375.5325130168 -2428459.4355383017, -6254383.86031451 -2428484.435538304, -6254392.1881159982 -2428509.4355383022, -6254394.0507979095 -2428515.0272959573, -6254400.515917493 -2428534.4355383036, -6254408.8437189814 -2428559.4355383026, -6254417.1715204753 -2428584.4355383003, -6254419.0507979095 -2428590.0771155925, -6254425.4993219655 -2428609.4355382971, -6254433.8271234594 -2428634.4355382989, -6254442.1549249552 -2428659.4355382989, -6254444.0507979048 -2428665.126935224, -6254450.4827264436 -2428684.435538298, -6254458.8105279356 -2428709.4355382957, -6254467.138329424 -2428734.4355382966, -6254469.050797902 -2428740.1767548565, -6254475.4661309179 -2428759.4355382994, -6254483.7939324081 -2428784.435538291, -6254488.1171720307 -2428797.4138724436, -6254494.0507978983 -2428787.6517201238, -6254496.0056557087 -2428784.4355382938, -6254511.2011414906 -2428759.4355382947, -6254519.0507979058 -2428746.521083768, -6254526.396627279 -2428734.4355383012, -6254519.0507979058 -2428734.4355382971, -6254526.39662728 -2428734.4355382989, -6254541.5921130609 -2428709.4355382994, -6254544.05079791 -2428705.3904474066, -6254556.7875988465 -2428684.4355383022, -6254569.0507979142 -2428664.2598110475, -6254571.98308463 -2428659.4355383059, -6254587.178570414 -2428634.4355383134, -6254594.0507979151 -2428623.1291746926, -6254602.3740561949 -2428609.4355383073, -6254617.5695419768 -2428584.43553831, -6254619.0507979207 -2428581.9985383344, -6254632.7650277568 -2428559.4355383161, -6254632.7650277615 -2428559.4355383161, -6254644.0507979263 -2428540.867901979, -6254647.9605135452 -2428534.4355383152, -6254663.1559993327 -2428509.4355383157, -6254669.0507979244 -2428499.7372656157, -6254678.3514851155 -2428484.4355383227, -6254693.5469708946 -2428459.4355383213, -6254694.0507979318 -2428458.6066292617, -6254694.0507979346 -2428434.4355383264, -6254708.7424566783 -2428434.435538318, -6254719.0507979384 -2428417.4759929045, -6254723.9379424639 -2428409.4355383255, -6254739.1334282458 -2428384.435538332, -6254744.0507979393 -2428376.3453565422, -6254753.3077126583 -2428361.11564482)), ((-6254708.7424566783 -2428434.4355383241, -6254694.0507979346 -2428434.4355383264, -6254694.0507979328 -2428458.6066292622, -6254708.7424566783 -2428434.4355383241)), ((-6254399.2597350832 -2428943.6043894938, -6254394.0507978816 -2428943.5478406339, -6254394.0507978844 -2428952.1742655579, -6254399.2597350832 -2428943.6043894938)), ((-6254369.05079788 -2428993.3049019151, -6254374.4417694416 -2428984.435538277, -6254369.0507978806 -2428984.435538278, -6254374.4417694416 -2428984.4355382761, -6254389.6372552253 -2428959.4355382812, -6254394.0507978816 -2428952.1742655588, -6254394.0507978816 -2428943.5478406339, -6254369.05079788 -2428943.2764375918, -6254344.0507978778 -2428943.0050345478, -6254319.05079788 -2428942.7336315047, -6254294.0507978769 -2428942.4622284621, -6254269.0507978741 -2428942.1908254218, -6254244.05079787 -2428941.9194223764, -6254219.0507978722 -2428941.648019332, -6254194.0507978741 -2428941.3766162931, -6254169.0507978676 -2428941.1052132491, -6254144.0507978639 -2428940.8338102074, -6254119.0507978648 -2428940.5624071644, -6254094.0507978611 -2428940.2910041185, -6254073.3327726927 -2428940.0660867151, -6254081.0446049273 -2428959.4355382626, -6254090.9982066322 -2428984.4355382607, -6254094.05079786 -2428992.1025901404, -6254100.951808339 -2429009.4355382593, -6254110.9054100439 -2429034.4355382589, -6254119.0507978583 -2429054.8939312571, -6254120.859011746 -2429059.4355382561, -6254130.8126134519 -2429084.4355382607, -6254140.5877170712 -2429108.987212894, -6254144.0507978536 -2429107.718292403, -6254169.0507978592 -2429098.557948139, -6254194.05079786 -2429089.397603869, -6254207.59304438 -2429084.4355382607, -6254219.0507978648 -2429080.237259604, -6254244.0507978629 -2429071.0769153368, -6254269.0507978685 -2429061.9165710746, -6254275.8219211269 -2429059.435538271, -6254294.05079787 -2429052.7562268032, -6254319.05079787 -2429043.5958825387, -6254344.0507978722 -2429034.4355382724, -6254359.246283656 -2429009.4355382756, -6254369.05079788 -2428993.3049019151)))");
            Assert.AreEqual(true, g2.IsValid);
            Assert.Throws<TopologyException>(() => g1.Difference(g2));
        }
    }
}