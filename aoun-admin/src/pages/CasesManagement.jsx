import { useState, useEffect } from 'react';
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
                                        <div className="bg-teal-600 h-2.5 rounded-full" style={{ width: `${progress}%` }}></div>
                                    </div>
                                    <span className="text-xs text-gray-500 font-bold">{progress}%</span>
                                </td>
                                <td className="p-4">
                                    <span className={`px-3 py-1 rounded-full text-xs font-bold ${c.status === 'مكتمل' ? 'bg-green-100 text-green-700' : 'bg-blue-100 text-blue-700'}`}>
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
}