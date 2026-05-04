angular.module('catApp')
.controller('AdminController', function($scope, CatModel) {
    $scope.cats = [];
    $scope.szures = { nev: '', szin: '', feltetel: '==', kor: '' };
    
    // Segédobjektumok a modálisokhoz
    $scope.editableCat = {}; 
    $scope.newCat = { nev: '', szin: '', kor: 0 };

    function refresh() {
        CatModel.loadCats().then(data => { $scope.cats = data; });
    }

    $scope.kombinaltSzuro = function(cat) {
        // 1. Alap szűrés névre és színre (ha nincs kitöltve, minden átmegy)
        const nevEgyezik = !$scope.szures.nev || 
            cat.nev.toLowerCase().includes($scope.szures.nev.toLowerCase());
        const szinEgyezik = !$scope.szures.szin || 
            cat.szin.toLowerCase().includes($scope.szures.szin.toLowerCase());

        // 2. Kor szűrése a kiválasztott operátorral
        let korEgyezik = true;
        if ($scope.szures.kor !== '' && $scope.szures.kor !== undefined) {
            const beirtKor = Number($scope.szures.kor);
            const macskaKora = Number(cat.kor);

            switch ($scope.szures.feltetel) {
                case '==': korEgyezik = (macskaKora === beirtKor); break;
                case '>':  korEgyezik = (macskaKora > beirtKor); break;
                case '>=': korEgyezik = (macskaKora >= beirtKor); break;
                case '<':  korEgyezik = (macskaKora < beirtKor); break;
                case '<=': korEgyezik = (macskaKora <= beirtKor); break;
                case '!=': korEgyezik = (macskaKora !== beirtKor); break;
            }
        }

        return nevEgyezik && szinEgyezik && korEgyezik;
    };

    // CREATE
    $scope.addCat = function() {
        CatModel.saveCat($scope.newCat).then(() => {
            refresh();
            $scope.newCat = { nev: '', szin: '', kor: 0 }; // Reset
        });
    };

    // UPDATE - adatok betöltése a szerkesztőbe
    $scope.prepareEdit = function(cat) {
        $scope.editableCat = angular.copy(cat); // Másolatot készítünk, hogy ne azonnal módosuljon a táblázatban
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