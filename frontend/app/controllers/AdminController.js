angular.module('catApp')
.controller('AdminController', function($scope, $location, $interval, CatModel) {
    $scope.cats = [];
    $scope.szures = { nev: '', szin: '', korFeltetel: '==', kor: '', arFeltetel: '==', ar: '' };
    $scope.userEmail = localStorage.getItem('felhasznalo') || "";

    // Segédobjektumok a modálisokhoz
    $scope.editableCat = {}; 
    $scope.newCat = { nev: '', szin: '', kor: 0, ar: 0 };

    function frissitHatralevoIdo() {
        const lejaratStr = localStorage.getItem("lejaratiIdopont");
        
        if (!lejaratStr) {
            $scope.hatralevoIdo = "Nincs adat";
            return;
        }

        const lejarat = new Date(lejaratStr);
        const most = new Date();
        const kulonbseg = lejarat - most;

        if (kulonbseg <= 0) {
            $scope.hatralevoIdo = "Lejárt!";
            $scope.logout();
            return;
        }

        const orak = Math.floor(kulonbseg / (1000 * 60 * 60));
        const percek = Math.floor((kulonbseg % (1000 * 60 * 60)) / (1000 * 60));
        const masodpercek = Math.floor((kulonbseg % (1000 * 60)) / 1000);

        $scope.hatralevoIdo = 
            (orak < 10 ? '0' : '') + orak + ':' + 
            (percek < 10 ? '0' : '') + percek + ':' + 
            (masodpercek < 10 ? '0' : '') + masodpercek;
    }

    $scope.logout = function() {
        localStorage.removeItem("token");
        localStorage.removeItem("felhasznalo");
        localStorage.removeItem("lejaratiIdopont");
        CatModel.deleteCart();

        $location.path('/login');
    };

    frissitHatralevoIdo();
    const timer = $interval(frissitHatralevoIdo, 1000);

    function refresh() {
        CatModel.loadCats().then(data => { $scope.cats = data; });
    }

    $scope.goToShop = function() {
        $location.path('/shop');
    };

    $scope.kombinaltSzuro = function(cat) {
        // 1. Alap szűrés névre és színre (ha nincs kitöltve, minden átmegy)
        const nevEgyezik = !$scope.szures.nev || 
            cat.nev.toLowerCase().includes($scope.szures.nev.toLowerCase());
        const szinEgyezik = !$scope.szures.szin || 
            cat.szin.toLowerCase().includes($scope.szures.szin.toLowerCase());

        // 2. Kor szűrése a kiválasztott operátorral
        let korEgyezik = true;
        if ($scope.szures.kor) {
            const beirtKor = Number($scope.szures.kor);
            const macskaKora = Number(cat.kor);
            
            switch ($scope.szures.korFeltetel) {
                case '==': korEgyezik = (macskaKora === beirtKor); break;
                case '>':  korEgyezik = (macskaKora > beirtKor); break;
                case '>=': korEgyezik = (macskaKora >= beirtKor); break;
                case '<':  korEgyezik = (macskaKora < beirtKor); break;
                case '<=': korEgyezik = (macskaKora <= beirtKor); break;
                case '!=': korEgyezik = (macskaKora !== beirtKor); break;
            }
        }
        
        // 3. Ár szűrése a kiválasztott operátorral
        let arEgyezik = true;
        if ($scope.szures.ar) {
            const macskaAra = Number(cat.ar);
            const beirtAr = Number($scope.szures.ar);
            
            switch ($scope.szures.arFeltetel) {
                case '==': arEgyezik = (macskaAra === beirtAr); break;
                case '>':  arEgyezik = (macskaAra > beirtAr); break;
                case '>=': arEgyezik = (macskaAra >= beirtAr); break;
                case '<':  arEgyezik = (macskaAra < beirtAr); break;
                case '<=': arEgyezik = (macskaAra <= beirtAr); break;
                case '!=': arEgyezik = (macskaAra !== beirtAr); break;
            }
        }

        return nevEgyezik && szinEgyezik && korEgyezik && arEgyezik;
    };

    // CREATE
    $scope.addCat = function() {
        CatModel.saveCat($scope.newCat).then(() => {
            refresh();
            $scope.newCat = { nev: '', szin: '', kor: 0, ar: 0 }; // Reset
        });
    };

    // UPDATE - adatok betöltése a szerkesztőbe
    $scope.prepareEdit = function(cat) {
        $scope.editableCat = angular.copy(cat);
    };

    $scope.saveEdit = function() {
        CatModel.updateCat($scope.editableCat).then(refresh);
    };

    // DELETE
    $scope.prepareDelete = function(cat) {
        $scope.targetDelete = cat;
    };

    $scope.confirmDelete = function() {
        CatModel.deleteCat($scope.targetDelete.id).then(refresh);
    };

    $scope.konfetti = function(cat) {
        const szovegek = [cat.nev, '🐈💨', 'ඞ', 'MEOW', '💩'];

        const duration = 2 * 1000;
        const end = Date.now() + duration;

        (function frame() {
            confetti({
                particleCount: 2,
                angle: 60,
                spread: 55,
                origin: { x: 0 },
                shapes: szovegek.map(s => confetti.shapeFromText({ text: s })),
                scalar: 3 
            });
            confetti({
                particleCount: 2,
                angle: 120,
                spread: 55,
                origin: { x: 1 },
                shapes: szovegek.map(s => confetti.shapeFromText({ text: s })),
                scalar: 3
            });

            if (Date.now() < end) {
                requestAnimationFrame(frame);
            }
        }());
    };

    refresh();
});