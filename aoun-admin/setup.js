import fs from 'fs';

console.log("🚀 جاري إضافة الصفحات المفقودة للوحة عون...");

// 1. صفحة إدارة الحالات والتبرعات (CasesManagement.jsx)
const casesJsx = `import { useState, useEffect } from 'react';
import { Heart, TrendingUp, AlertCircle } from 'lucide-react';

export default function CasesManagement() {
    const [cases] = useState([
        { id: 1, title: 'تجهيز عروسة يتيمة', category: 'الغارمين', required: 35000, collected: 15000, status: 'نشط' },
        { id: 2, title: 'جراحة قلب مفتوح', category: 'علاج', required: 120000, collected: 120000, status: 'مكتمل' },
        { id: 3, title: 'سقف يحمي أسرة', category: 'إعمار', required: 25000, collected: 5000, status: 'نشط' }
    ]);

    return (
        <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
            <div className="flex items-center gap-3 mb-6 border-b pb-4">
                <Heart className="text-teal-600 w-8 h-8" />
                <h3 className="text-xl font-bold">مراقبة الحالات والتبرعات (Live Data)</h3>
            </div>
            <table className="w-full text-right">
                <thead className="bg-gray-50 text-gray-600 rounded-lg">
                    <tr>
                        <th className="p-4 font-bold rounded-r-lg">عنوان الحالة</th>
                        <th className="p-4 font-bold">التصنيف</th>
                        <th className="p-4 font-bold">المطلوب (ج.م)</th>
                        <th className="p-4 font-bold">المُجمّع (ج.م)</th>
                        <th className="p-4 font-bold">نسبة الإنجاز</th>
                        <th className="p-4 font-bold rounded-l-lg">الحالة</th>
                    </tr>
                </thead>
                <tbody>
                    {cases.map(c => {
                        const progress = Math.round((c.collected / c.required) * 100);
                        return (
                            <tr key={c.id} className="border-b hover:bg-gray-50 transition">
                                <td className="p-4 font-bold text-gray-800">{c.title}</td>
                                <td className="p-4 text-gray-500">{c.category}</td>
                                <td className="p-4 font-semibold text-gray-700">{c.required.toLocaleString()}</td>
                                <td className="p-4 font-bold text-teal-600">{c.collected.toLocaleString()}</td>
                                <td className="p-4">
                                    <div className="w-full bg-gray-200 rounded-full h-2.5 mt-2">
                                        <div className="bg-teal-600 h-2.5 rounded-full" style={{ width: \`\${progress}%\` }}></div>
                                    </div>
                                    <span className="text-xs text-gray-500 font-bold">{progress}%</span>
                                </td>
                                <td className="p-4">
                                    <span className={\`px-3 py-1 rounded-full text-xs font-bold \${c.status === 'مكتمل' ? 'bg-green-100 text-green-700' : 'bg-blue-100 text-blue-700'}\`}>
                                        {c.status}
                                    </span>
                                </td>
                            </tr>
                        );
                    })}
                </tbody>
            </table>
        </div>
    );
}`;
fs.writeFileSync('./src/pages/CasesManagement.jsx', casesJsx);

// 2. صفحة الإعدادات (Settings.jsx)
const settingsJsx = `import { Settings, Save, Bell, Shield } from 'lucide-react';

export default function AppSettings() {
    return (
        <div className="max-w-4xl mx-auto bg-white p-8 rounded-2xl shadow-sm border border-gray-100">
            <div className="flex items-center gap-3 mb-8 border-b pb-4">
                <Settings className="text-teal-600 w-8 h-8" />
                <h3 className="text-2xl font-bold">إعدادات المنصة المركزية</h3>
            </div>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
                <div className="space-y-4">
                    <h4 className="font-bold flex items-center gap-2 text-gray-700"><Shield size={18}/> سياسات الأمان</h4>
                    <div className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                        <span className="font-semibold text-sm">تفعيل المصادقة الثنائية (2FA) للمديرين</span>
                        <input type="checkbox" className="w-5 h-5 text-teal-600" defaultChecked />
                    </div>
                    <div className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                        <span className="font-semibold text-sm">إيقاف تسجيل الجمعيات الجديدة مؤقتاً</span>
                        <input type="checkbox" className="w-5 h-5 text-teal-600" />
                    </div>
                </div>

                <div className="space-y-4">
                    <h4 className="font-bold flex items-center gap-2 text-gray-700"><Bell size={18}/> الإشعارات</h4>
                    <div className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                        <span className="font-semibold text-sm">تنبيه عند تخطي التبرعات 100,000 ج.م</span>
                        <input type="checkbox" className="w-5 h-5 text-teal-600" defaultChecked />
                    </div>
                    <div className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                        <span className="font-semibold text-sm">إرسال تقرير يومي للمدير العام</span>
                        <input type="checkbox" className="w-5 h-5 text-teal-600" defaultChecked />
                    </div>
                </div>
            </div>

            <button className="w-full bg-gradient-to-r from-teal-600 to-teal-800 text-white font-bold p-4 rounded-xl hover:shadow-lg transition-all flex justify-center items-center gap-2">
                <Save size={20} /> حفظ الإعدادات
            </button>
        </div>
    );
}`;
fs.writeFileSync('./src/pages/Settings.jsx', settingsJsx);

// 3. تحديث مسارات App.jsx لدمج الصفحات الجديدة
const appJsx = fs.readFileSync('./src/App.jsx', 'utf-8');
const updatedAppJsx = appJsx
    .replace("import AdminsManagement from './pages/AdminsManagement';", "import AdminsManagement from './pages/AdminsManagement';\nimport CasesManagement from './pages/CasesManagement';\nimport AppSettings from './pages/Settings';")
    .replace('<Route path="admins" element={<AdminsManagement />} />', '<Route path="admins" element={<AdminsManagement />} />\n          <Route path="cases" element={<CasesManagement />} />\n          <Route path="settings" element={<AppSettings />} />');
fs.writeFileSync('./src/App.jsx', updatedAppJsx);

console.log("✅ تمت إضافة صفحات الحالات والإعدادات بنجاح! اعمل Refresh للمتصفح.");