import { Settings, Save, Bell, Shield } from 'lucide-react';

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
}