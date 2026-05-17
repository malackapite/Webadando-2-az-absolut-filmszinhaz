angular.module('catApp')
.controller('StepperController', function($http, $scope, OrderModel, CatModel, $timeout) {
    $scope.currentStep = 1; // 1: Kosár, 2: Szállítás, 3: Fizetés, 4: Siker
    $scope.termekek = [];
    $scope.szallitasiModok = OrderModel.getSzallitasiModok();
    $scope.fizetesiModok = OrderModel.getFizetesiModok();

    $scope.rendelesLeadas = function() {
        
        return $http.post('https://localhost:7063/rendeles', {
            szallitasiCim: $scope.form.vevo.cim,
            rendeltMacskak: $scope.termekek.map(item => ({ 
                macska: item.adat.id, 
                mennyiseg: item.db 
            })),    
        }).then(resp => resp.data)
        .catch(err => {
            console.error("Hiba a rendelés leadásakor:", err);
        });
    };

    // Felhasználói adatok objektuma
    $scope.form = {
        szallitas: $scope.szallitasiModok[0],
        fizetes: $scope.fizetesiModok[0],
        vevo: { nev: '', email: '', cim: '', irsz: '' },
        kartya: { szam: '', lejarat: '', cvv: '' }
    };

    $scope.kosarFrissit = function() {
        const kosarIDk = CatModel.getCart(); 
        
        $scope.reszletesKosar = [];

        angular.forEach(kosarIDk, function(db, id) {
            if (db > 0) {
                // Mindig frissen kérjük le a CatModelltől
                CatModel.findCatById(id).then(function(cat) {
                    if (cat) {
                        $scope.reszletesKosar.push({
                            adat: cat,
                            db: db
                        });
                    }
                });
            }
            $scope.termekek = $scope.reszletesKosar; 
        });
    };

    $scope.reszosszeg = function() {
        return $scope.termekek.reduce((s, t) => s + (t.adat.ar * t.db), 0);
    };

    $scope.vegosszeg = function() {
        return $scope.reszosszeg() + $scope.form.szallitas.ar;
    };

    $scope.nextStep = function() {
        if ($scope.currentStep < 4) $scope.currentStep++;
    };

    $scope.prevStep = function() {
        if ($scope.currentStep > 1) $scope.currentStep--;
    };

    $scope.finishOrder = function() {
        $scope.loading = true;
        this.rendelesLeadas().then(() => {
            $scope.loading = false;
            $scope.currentStep = 4;
        })
        .catch(() => {
            $scope.loading = false;
            alert("Hiba történt a rendelés leadásakor. Kérem, próbálja újra.");
        });
        CatModel.deleteCart();
    };

    $scope.kosarFrissit();
});