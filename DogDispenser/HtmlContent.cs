namespace DogDispenser
{
    public static class HtmlContent
    {
        public const string HomePage = @"<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width,initial-scale=1.0'>
    <title>Dog Dispenser</title>
    <style>
        body {
            font-family: Arial,sans-serif;
            max-width: 600px;
            margin: 50px auto;
            padding: 20px;
            background: linear-gradient(135deg,#667eea 0%,#764ba2 100%);
            color: #fff
        }

        .container {
            background: rgba(255,255,255,0.1);
            padding: 30px;
            border-radius: 15px;
            backdrop-filter: blur(10px)
        }

        h1 {
            text-align: center;
            margin-bottom: 30px
        }

        h3 {
            margin-top: 25px;
            margin-bottom: 10px
        }

        button {
            width: 100%;
            padding: 15px;
            margin: 10px 0;
            font-size: 18px;
            border: none;
            border-radius: 10px;
            cursor: pointer;
            background: #fff;
            color: #667eea;
            font-weight: bold;
            transition: all 0.3s
        }

            button:hover {
                transform: scale(1.05);
                box-shadow: 0 5px 15px rgba(0,0,0,0.3)
            }

        .dispense {
            background: #ff6b6b;
            color: #fff;
            padding: 25px;
            font-size: 24px
        }

        input {
            width: 100%;
            padding: 10px;
            margin: 10px 0;
            border-radius: 5px;
            border: none;
            font-size: 16px;
            box-sizing: border-box
        }

        input[type='time'] {
            width: calc(70% - 5px);
            display: inline-block
        }

        input[type='checkbox'] {
            width: auto;
            margin-left: 10px;
            transform: scale(1.5)
        }

        .status {
            text-align: center;
            padding: 15px;
            margin: 20px 0;
            background: rgba(255,255,255,0.2);
            border-radius: 10px
        }

        .schedule-row {
            display: flex;
            align-items: center;
            margin: 10px 0;
            gap: 10px
        }

        .schedule-label {
            font-size: 14px;
            margin-bottom: 5px
        }

        .btn-small {
            padding: 10px;
            font-size: 16px
        }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🐕 Dog Dispenser</h1>
        <div class='status' id='status'>Ready</div>
        
        <button class='dispense' onclick='dispense()'>🦴 DISPENSE NOW</button>
        
        <h3>⏰ Automatic Schedule</h3>
        <div class='schedule-label'>Schedule 1:</div>
        <div class='schedule-row'>
            <input type='time' id='time1' value='08:00'>
            <label><input type='checkbox' id='enabled1'> On</label>
        </div>
        <button class='btn-small' onclick='saveSchedule1()'>💾 Save Schedule 1</button>
        
        <div class='schedule-label' style='margin-top:15px'>Schedule 2:</div>
        <div class='schedule-row'>
            <input type='time' id='time2' value='18:00'>
            <label><input type='checkbox' id='enabled2'> On</label>
        </div>
        <button class='btn-small' onclick='saveSchedule2()'>💾 Save Schedule 2</button>
        
        <h3>Manual Control</h3>
        <button onclick='forward()'>⬆️ Forward</button>
        <button onclick='backward()'>⬇️ Backward</button>
    </div>
    <script>
        function s(m){document.getElementById('status').innerHTML=m}
        
        function dispense(){
            s('🦴 Dispensing...');
            fetch('/dispense').then(()=>s('✅ Done!')).catch(()=>s('❌ Error'))
        }
        
        function forward(){
            s('⬆️ Forward...');
            fetch('/forward').then(()=>s('✅ Done!')).catch(()=>s('❌ Error'))
        }
        
        function backward(){
            s('⬇️ Backward...');
            fetch('/backward').then(()=>s('✅ Done!')).catch(()=>s('❌ Error'))
        }
        
        function saveSchedule1(){
            var time=document.getElementById('time1').value.split(':');
            var enabled=document.getElementById('enabled1').checked?1:0;
            s('💾 Saving...');
            fetch('/setSchedule1?hour='+time[0]+'&minute='+time[1]+'&enabled='+enabled)
                .then(()=>s('✅ Schedule 1 saved!'))
                .catch(()=>s('❌ Error'))
        }
        
        function saveSchedule2(){
            var time=document.getElementById('time2').value.split(':');
            var enabled=document.getElementById('enabled2').checked?1:0;
            s('💾 Saving...');
            fetch('/setSchedule2?hour='+time[0]+'&minute='+time[1]+'&enabled='+enabled)
                .then(()=>s('✅ Schedule 2 saved!'))
                .catch(()=>s('❌ Error'))
        }
        
        function loadSchedules(){
            fetch('/getSchedules')
                .then(r=>r.text())
                .then(data=>{
                    var json=JSON.parse(data);
                    var s1=json.schedule1.split('|');
                    var s2=json.schedule2.split('|');
                    document.getElementById('time1').value=s1[0];
                    document.getElementById('enabled1').checked=s1[1]=='1';
                    document.getElementById('time2').value=s2[0];
                    document.getElementById('enabled2').checked=s2[1]=='1';
                })
                .catch(()=>console.log('Error loading schedules'))
        }
        
        <!-- window.onload=loadSchedules; -->
    </script>
</body>
</html>";
    }
}