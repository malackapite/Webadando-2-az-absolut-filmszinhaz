angular.module('catApp')
.controller('CatController', function($scope, CatModel, $location, $timeout, $interval) {
    $scope.cats = [];
    $scope.nth = 0;
    $scope.vasaroltDb = 1;
    $scope.userEmail = localStorage.getItem('felhasznalo') || "";
    let player = null;

    CatModel.loadCats().then(data => {
        $scope.cats = data;
        $timeout(init3D, 100);
    });

    function init3D() {
        $timeout(function() {
            const container = document.getElementById("macska3D");
            
            if (!container || !window.APP || !window.THREE) {
                console.error("Three.js vagy a konténer nem található!");
                return;
            }

            const loader = new window.THREE.FileLoader();
            loader.load('app.json', function (text) {
                // Új player példány
                player = new window.APP.Player();
                player.load(JSON.parse(text));
                
                // Méretezés a div szélességéhez
                player.setSize(container.clientWidth, 400);
                player.play();
                if (player && typeof player.setupCats === 'function') {
                    player.setupCats($scope.cats.length);
                }
                
                container.innerHTML = ""; // Ürítjük a konténert az újratöltés megelőzésére
                container.appendChild(player.dom);
                
                // Kezdeti állapot átadása a Three.js-nek
                if (typeof player.setNth === 'function') {
                    player.setNth($scope.nth);
                }
            });
        }, 100);
    }

    $scope.$on('$destroy', function() {
        $interval.cancel(timer);
        if (player) {
            player.stop(); 
            player.dispose(); 
        }
    });

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

        $location.path('/login');
    };

    frissitHatralevoIdo();
    const timer = $interval(frissitHatralevoIdo, 1000);

    $scope.getLathatoGombok = function() {
    const maxGomb = 5;
    const osszesen = $scope.cats.length;
    
    // Ha kevesebb macska van, mint 5, akkor az összeset mutatjuk
    if (osszesen <= maxGomb) {
        return $scope.cats.map((c, index) => index);
    }

    // Kiszámoljuk a start pozíciót, hogy az 'nth' lehetőleg középen legyen
    let start = $scope.nth - Math.floor(maxGomb / 2);
    
    // Biztonsági korrekciók, hogy ne csússzunk ki a tömbből
    if (start < 0) {
        start = 0;
    }
    if (start + maxGomb > osszesen) {
        start = osszesen - maxGomb;
    }

    // Legyártjuk a fix 5 darab indexet tartalmazó tömböt (pl: [2, 3, 4, 5, 6])
    let gombok = [];
    for (let i = start; i < start + maxGomb; i++) {
        gombok.push(i);
    }
    return gombok;
};

    $scope.getLength = function() {
        return $scope.cats.length;
    };

    // Lapozás
    $scope.lapoz = function(irany) {
        let ujIndex = $scope.nth + irany;
        if (ujIndex >= 0 && ujIndex < $scope.cats.length) {
            $scope.nth = ujIndex;
            $scope.vasaroltDb = 1;
            if (player) player.setNth($scope.nth); // 3D modell frissítése
        }
    };

    $scope.ugrik = function(index) {
        $scope.nth = index;
        $scope.vasaroltDb = 1;
        if (player) player.setNth($scope.nth); // 3D modell frissítése
    };

    // Kosár kezelés
    $scope.kosarlista = CatModel.getCart();

    $scope.vasarol = function() {
        let currentCat = $scope.cats[$scope.nth];
        if ($scope.vasaroltDb > 0 && $scope.vasaroltDb < 100) {
            CatModel.addToCart(currentCat.id, $scope.vasaroltDb);
            new bootstrap.Toast(document.getElementById('sikeresVasarlas')).show();
        } else {
            new bootstrap.Toast(document.getElementById('liveToast')).show();
        }
    };

    $scope.getCatById = function(id) {
        return $scope.cats.find(c => c.id == id);
    };

    $scope.getKosarDb = function() {
        return Object.keys($scope.kosarlista).length;
    };

    $scope.torol = function(id) {
        CatModel.removeItem(id);
    };

    $scope.fizetes = function() {
        $('#kosarPanel').modal('hide');
        $location.path('/checkout');
    };
});