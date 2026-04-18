import * as THREE from './three.module.js';

var APP = {
	
	Player: function () {
		
		var renderer = new THREE.WebGLRenderer({
			antialias: true,
			alpha: true,
			premultipliedAlpha: false
		});
		renderer.setPixelRatio(window.devicePixelRatio); // TODO: Use player.setPixelRatio()
		renderer.outputEncoding = THREE.sRGBEncoding;
		
		var loader = new THREE.ObjectLoader();
		var camera, scene;

		var vrButton = VRButton.createButton(renderer); // eslint-disable-line no-undef
		
		var events = {};
		
		var dom = document.createElement('div');
		dom.appendChild(renderer.domElement);
		
		this.dom = dom;
		
		this.width = 500;
		this.height = 500;
		
		const fingIdo = new THREE.Clock();
		const idoCam = new THREE.Clock();
		const raycaster = new THREE.Raycaster();
		const clickMouse = new THREE.Vector2();
		
		let nth = [0, 0]; // nth[0] a jelenlegi, nth[1] a következő index
		const distance = 1.5;

		this.setNth = function (value) {
				idoCam.start() // Kamera animáció újraindítása
				nth[0] = nth[1]; // Előző érték átmentése
				nth[1] = value; 
        };

		const onMouseClick = function (event) {
			if (!scene || !camera) return;

			const rect = renderer.domElement.getBoundingClientRect();
			clickMouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
			clickMouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;
			raycaster.setFromCamera(clickMouse, camera);
			const intersects = raycaster.intersectObjects(scene.children, true);
			if (intersects.length > 0) {
				const foundObject = intersects[0].object;
				
				if (foundObject.name.includes("fart")) {
					
					const masik = foundObject.parent.clone();
					
					fingIdo.start();
					
					// Hozzáadjuk a klónt a nagyszülőhöz
					if (foundObject.parent.parent) {
						foundObject.parent.parent.add(masik);
					}
					
					foundObject.parent.clear();
					new Audio('fart.mp3').play();
				}
			}
			
		};

		this.load = function (json) {

			var project = json.project;

			if (project.vr !== undefined) renderer.xr.enabled = project.vr;
			if (project.shadows !== undefined) renderer.shadowMap.enabled = project.shadows;
			if (project.shadowType !== undefined) renderer.shadowMap.type = project.shadowType;
			if (project.toneMapping !== undefined) renderer.toneMapping = project.toneMapping;
			if (project.toneMappingExposure !== undefined) renderer.toneMappingExposure = project.toneMappingExposure;
			if (project.physicallyCorrectLights !== undefined) renderer.physicallyCorrectLights = project.physicallyCorrectLights;

			this.setScene(loader.parse(json.scene));
			this.setCamera(loader.parse(json.camera));

			events = {
				init: [],
				start: [],
				stop: [],
				keydown: [],
				keyup: [],
				pointerdown: [],
				pointerup: [],
				pointermove: [],
				update: []
			};

			var scriptWrapParams = 'player,renderer,scene,camera';
			var scriptWrapResultObj = {};

			for (var eventKey in events) {

				scriptWrapParams += ',' + eventKey;
				scriptWrapResultObj[eventKey] = eventKey;

			}

			var scriptWrapResult = JSON.stringify(scriptWrapResultObj).replace(/\"/g, '');

			for (var uuid in json.scripts) {

				var object = scene.getObjectByProperty('uuid', uuid, true);

				if (object === undefined) {

					console.warn('APP.Player: Script without object.', uuid);
					continue;

				}

				var scripts = json.scripts[uuid];

				for (var i = 0; i < scripts.length; i++) {

					var script = scripts[i];

					var functions = (new Function(scriptWrapParams, script.source + '\nreturn ' + scriptWrapResult + ';').bind(object))(this, renderer, scene, camera);

					for (var name in functions) {

						if (functions[name] === undefined) continue;

						if (events[name] === undefined) {

							console.warn('APP.Player: Event type not supported (', name, ')');
							continue;

						}

						events[name].push(functions[name].bind(object));

					}

				}

			}

			dispatch(events.init, arguments);
			this.setupCats = function (count) {
				if (!scene) return;

				const baseCat = scene.getObjectByName("node_id34");
				if (!baseCat) return;

				const firstFart = scene.getObjectByName("fart");
				if (firstFart) firstFart.name = "fart0";

				for (let ix = 0; ix < count - 1; ix++) {
					var clone = baseCat.clone();
					
					clone.name = "macska" + (ix + 1); // Azonosító az animációhoz
					clone.position.x = distance + (distance * ix);
					
					if (clone.children[1] && clone.children[1].children[0]) {
						clone.children[1].children[0].name = "fart" + (ix + 1);
					}
					
					scene.add(clone);
				}
			};	
		};
		

		this.setCamera = function (value) {

			camera = value;
			camera.aspect = this.width / this.height;
			camera.updateProjectionMatrix();

		};

		this.setScene = function (value) {

			scene = value;

		};

		this.setPixelRatio = function (pixelRatio) {

			renderer.setPixelRatio(pixelRatio);

		};

		this.setSize = function (width, height) {

			this.width = width;
			this.height = height;

			if (camera) {

				camera.aspect = this.width / this.height;
				camera.updateProjectionMatrix();

			}

			renderer.setSize(width, height);

		};

		function dispatch(array, event) {

			for (var i = 0, l = array.length; i < l; i++) {

				array[i](event);

			}

		}

		var time, startTime, prevTime;

		function animate() {

			time = performance.now();

			try {

				dispatch(events.update, { time: time - startTime, delta: time - prevTime });

			} catch (e) {

				console.error((e.message || e), (e.stack || ''));

			}

			const delta = fingIdo.getElapsedTime();

			// Csak akkor futtatjuk a skálázást, ha az animáció aktív (1 másodpercen belül)
			if (delta > 0 && delta < 1) {
				const scaleValue = (Math.pow(delta, 2) + Math.sin(delta * 3)) / 1.052335956;

				// Végigmegyünk a jelenet gyerekein, és megkeressük azt, amelyik éppen "fart"
				scene.traverse(function (child) {
					if (child.name && child.name.includes("fart")) {
						child.scale.set(scaleValue, scaleValue, scaleValue);
					}
				});
			} else if (delta >= 1) {
				fingIdo.stop();
			}
			
			if (idoCam.getElapsedTime() < 1) {
				camera.position.x = nth[0] * distance + (nth[1] - nth[0]) * (Math.pow((idoCam.getElapsedTime()), 2)) / (Math.pow(1, 2)) * distance
			} else {
				camera.position.x = nth[0] * distance + (nth[1] - nth[0]) * distance

			}


			scene.children.forEach(child => {
				if (child.name?.startsWith("macska")) {
					child.rotation.y = (time * 0.001) * -0.5;
				}
			});

			renderer.render(scene, camera);

			prevTime = time;

		}

		this.play = function () {

			if (renderer.xr.enabled) dom.append(vrButton);

			startTime = prevTime = performance.now();

			document.addEventListener('keydown', onKeyDown);
			document.addEventListener('keyup', onKeyUp);
			document.addEventListener('pointerdown', onPointerDown);
			document.addEventListener('pointerup', onPointerUp);
			document.addEventListener('pointermove', onPointerMove);

			dispatch(events.start, arguments);

			renderer.domElement.addEventListener("click", onMouseClick);
			renderer.setAnimationLoop(animate);

		};

		this.stop = function () {

			if (renderer.xr.enabled) vrButton.remove();

			document.removeEventListener('keydown', onKeyDown);
			document.removeEventListener('keyup', onKeyUp);
			document.removeEventListener('pointerdown', onPointerDown);
			document.removeEventListener('pointerup', onPointerUp);
			document.removeEventListener('pointermove', onPointerMove);

			dispatch(events.stop, arguments);

			renderer.domElement.removeEventListener("click", onMouseClick);
			renderer.setAnimationLoop(null);

		};

		this.render = function (time) {

			dispatch(events.update, { time: time * 1000, delta: 0 /* TODO */ });

			renderer.render(scene, camera);

		};

		this.dispose = function () {

			renderer.dispose();

			camera = undefined;
			scene = undefined;

		};


		function onKeyDown(event) {

			dispatch(events.keydown, event);

		}

		function onKeyUp(event) {

			dispatch(events.keyup, event);

		}

		function onPointerDown(event) {

			dispatch(events.pointerdown, event);

		}

		function onPointerUp(event) {

			dispatch(events.pointerup, event);

		}

		function onPointerMove(event) {

			dispatch(events.pointermove, event);

		}
	}

};

export { APP };
